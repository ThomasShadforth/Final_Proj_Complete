using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleClassAbilities : MonoBehaviour
{
    #region Variables
    //Strings to hold the selected node for each group
    [Header("Selected Abilities")]
    public string selectedJump;
    public string selectedDodge;
    public string selectedBuff;
    //insert fourth ability string here

    //Cooldowns for each ability type
    [Header("Ability Cooldowns")]
    public float maxBuffCooldown;
    public float buffCooldown;
    public float speedBuffCooldown;
    public float jumpBuffCooldown;
    public float maxDodgeCooldown;
    public float dodgeCooldown;
    public float sideStepCooldown;
    public float backPaceCooldown;

    //Timers for how long specific abilities last
    [Header("Ability Timers")]
    [SerializeField] float jumpTimer;
    public float rocketJumpTimer;
    public float gravJumpTimer;
    [SerializeField] float buffTimer;
    
    public float speedBuffTimer;
    public float jumpBuffTimer;

    //Properties for each of the abilities (Where applicable)
    [Header("Ability Properties")]
    public float rocketJumpHeight;
    public float ZeroGravityDuration;
    public float sideStepDistance;
    public float backPaceDistance;
    public float speedBuffMod = 1.2f;
    public float jumpBuffMod = 1.4f;
    public bool isDodging;
    public bool isSuperJump;

    //Read Only Arrays for the Abilities
    public readonly string[] dodgeAbilities = { "SideStepDodge", "BackPaceDodge" };
    public readonly string[] jumpAbilities = { "RocketJump", "ZeroGravJump", "Jump" };
    public readonly string[] buffAbilities = { "MoveSpeedBuff", "HeightBuff" };

    //player properties
    Rigidbody rb;
    Vector3 move;
    PlayerBase player;
    #endregion

    public string BuffUIString;
    public TextMeshProUGUI BuffUI;
    bool hasRemoved;
    bool buffTimerReached = true;
    bool dodgeTimerReached;
    public float superJumpLimit = 1;
    public float superJumpLimitVal;
    bool hasResetLimit = true;
    public List<float> timers = new List<float>();

    private void Start()
    {
        player = PlayerBase.instance;
        rb = GetComponent<Rigidbody>();
        superJumpLimitVal = superJumpLimit;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.paused)
        {
            return;
        }

        if(TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && !TutorialSystem.instance.doesTutHaveConditions))
        {
            return;
        }

        checkForAbilityPress();
        checkForAbilityTimers();
        checkForAbilityCooldowns();
        checkForGrounded();
        SetUI();

    }

    private void checkForGrounded()
    {
        if (player.isGrounded && !hasResetLimit && rb.velocity.y == 0)
        {
            
            hasResetLimit = true;
            superJumpLimitVal = superJumpLimit;
        }
    }

    private void FixedUpdate()
    {
        move.x = Input.GetAxisRaw("Horizontal");
    }

    #region Checks
    void checkForAbilityPress()
    {
        if (Input.GetButtonDown("Dodge"))
        {
            if(dodgeCooldown <= 0) {
                
                DodgeAbility();
            }
        }
        else if (Input.GetButtonDown("Buff"))
        {
            if(buffCooldown <= 0)
            {
                BuffAbility();
            }
        }

        else if (Input.GetButtonDown("SuperJump") && superJumpLimitVal == 1)
        {
            JumpAbility();
        }
    }

    void checkForAbilityTimers()
    {
        if(buffTimer > 0)
        {
            buffTimer -= GamePause.deltaTime;
            buffTimerReached = false;
        }
        else
        {
            switch (selectedBuff)
            {
                case "MoveSpeedBuff":
                    player.speedModifier = 1.0f;
                    break;
                case "HeightBuff":
                    player.jumpModifier = 1.0f;
                    break;
            }
            if (!buffTimerReached)
            {
                RemoveBuffFromUI(selectedBuff);
                buffTimerReached = true;
            }
        }
    }

    void checkForAbilityCooldowns()
    {
        if(buffCooldown > 0)
        {
            buffCooldown -= GamePause.deltaTime;
        }
        if(dodgeCooldown > 0)
        {
            dodgeCooldown -= GamePause.deltaTime;
        }
    }

    #endregion

    #region Dodge Abilities
    void DodgeAbility()
    {
        switch (selectedDodge)
        {
            case "SideStepDodge":
                StartCoroutine(SideStepDodgeAbility(move.x));
                break;
            case "BackPaceDodge":
                StartCoroutine(BackPaceDodgeAbility());
                break;
            default:
                break;
        }
    }

    IEnumerator SideStepDodgeAbility(float xMove)
    {
        if(xMove == 0)
        {
            xMove = 1;
        }

        isDodging = true;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);

        rb.AddForce((transform.right * xMove * sideStepDistance) * speedBuffMod, ForceMode.Impulse);
        yield return new WaitForSeconds(.3f);

        rb.velocity = Vector3.zero;
        isDodging = false;
        dodgeCooldown = sideStepCooldown;
        maxDodgeCooldown = sideStepCooldown;
    }

    IEnumerator BackPaceDodgeAbility()
    {
        isDodging = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce((-transform.forward * backPaceDistance) * speedBuffMod, ForceMode.Impulse);

        yield return new WaitForSeconds(.6f);

        rb.velocity = Vector3.zero;
        isDodging = false;
        dodgeCooldown = backPaceCooldown;
        maxDodgeCooldown = backPaceCooldown;
    }
    #endregion

    #region Buff Abilities
    void BuffAbility()
    {
        AddBuffToUI(selectedBuff);
        
        switch (selectedBuff)
        {

            case "MoveSpeedBuff":
                player.speedModifier = speedBuffMod;
                buffTimer = speedBuffTimer;
                buffCooldown = speedBuffCooldown;
                maxBuffCooldown = speedBuffCooldown;
                timers.Add(buffTimer);
                break;
            case "HeightBuff":
                player.jumpModifier = jumpBuffMod;
                buffTimer = jumpBuffTimer;
                buffCooldown = jumpBuffCooldown;
                maxBuffCooldown = jumpBuffCooldown;
                timers.Add(buffTimer);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Jump Abilities
    void JumpAbility()
    {
        hasResetLimit = false;
        superJumpLimitVal--;
        switch (selectedJump)
        {
            case "RocketJump":
                rb.velocity = Vector3.up * player.jumpForce * rocketJumpHeight * jumpBuffMod;
                player.speed = player.speed / 4;
                Invoke("ResetSpeed", 2.2f);
                break;
            case "ZeroGravJump":
                rb.useGravity = false;
                Invoke("ResetGravity", ZeroGravityDuration);
                break;
            case "Jump":
                rb.velocity = Vector3.up * player.jumpForce * jumpBuffMod;
                break;
            default:
                //No jump active, do nothing
                break;
        }

        
    }

    void ResetGravity()
    {
        rb.useGravity = true;
    }

    void ResetSpeed()
    {
        player.speed = player.defaultSpeed;
    }
    #endregion

    #region Ability Changes
    public void setAbility(string abilityToSet)
    {
        if (abilityToSet.Contains("Jump")){
            selectedJump = abilityToSet;
        } else if (abilityToSet.Contains("Dodge"))
        {
           
            selectedDodge = abilityToSet;
        } else if (abilityToSet.Contains("Buff"))
        {
            selectedBuff = abilityToSet;
        }

        PlayerUI.instance.UpdateGameplayAbilityUI();
    }
    #endregion

    #region Utility Methods
    public void SetUI()
    {
        BuffUI.text = "";

        if (BuffUIString != "")
        {
            string[] buffList = BuffUIString.Split(',');

            List<string> buffs = buffList.ToList<string>();

            foreach (string buff in buffs)
            {
                if (buff != "")
                {
                    BuffUI.text = BuffUI.text + buff + ": " + ReduceListTimer(BuffUIString, buff) + "\n";
                }
            }
        }
    }

    public void AddBuffToUI(string stringToAdd)
    {
        
        BuffUIString += $"{stringToAdd},";
    }

    public void RemoveBuffFromUI(string stringToRemove)
    {
        string[] buffList = BuffUIString.Split(',');

        List<string> buffs = buffList.ToList<string>();

        

        /*
        for(int i = 0; i < buffList.Length; i++)
        {
            if(buffList[i] == stringToRemove)
            {
                Debug.Log("FOUND!");
                buffList[i].Remove(0);

                Debug.Log(buffList.Length);

                buffList = ShiftBuffNames(buffList);
            }
        }*/

        for(int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i] == stringToRemove)
            {
                buffs.Remove(stringToRemove);

                buffs = ShiftBuffNames(buffs.ToArray<string>()).ToList<string>();
            }
            
        }

        
        BuffUIString = GetStringFromArray(buffs.ToArray<string>());
    }

    private string GetStringFromArray(string[] buffList)
    {
        string newBuffString;
        if (buffList.Length != 1)
        {
            newBuffString = string.Join(",", buffList);
        }
        else
        {
            newBuffString = "";
        }



        return newBuffString;
    }

    public string[] ShiftBuffNames(string[] names)
    {
        if (names.Length != 1 && names[0] != "")
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == "")
                {
                    if (i + 1 == names.Length)
                    {
                        break;
                    }
                    names[i] = names[i + 1];
                    names[i + 1] = "";
                }
                else
                {
                    continue;
                }
            }
        }

        return names;
    }

    public float ReduceListTimer(string buffString, string buffName)
    {
        if (timers.Count > 0)
        {
            string[] listOfBuffs = buffString.Split(',');

            for (int i = 0; i < listOfBuffs.Length; i++)
            {
                if (listOfBuffs[i] == buffName && buffName != "")
                {

                    timers[i] -= GamePause.deltaTime;

                    if (timers[i] <= 0)
                    {
                        timers.Remove(timers[i]);
                    }
                    return Mathf.Round(timers[i]);
                }
            }
        }

        return 0f;
    }

    #endregion
}
