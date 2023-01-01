using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SimpleClassAbilities : MonoBehaviour
{
    #region Variables
    //Strings to hold the name of the selected node for each group
    [Header("Selected Abilities")]
    public string selectedJump;
    public string selectedDodge;
    public string selectedBuff;
    

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
    //rocketJump height and duration of zero gravity
    public float rocketJumpHeight;
    public float ZeroGravityDuration;
    //Distance of the dodges
    public float sideStepDistance;
    public float backPaceDistance;
    //Value of the buff applied to move speed and jumpHeight (Along with their respective storage variables)
    public float speedBuffMod = 1.2f;
    float speedBuffVal = 1f;
    public float jumpBuffMod = 1.4f;
    float jumpBuffVal = 1f;
    //Whether or not the player is dodging, or using the rocketJump
    public bool isDodging;
    public bool isSuperJump;
    float defaultJumpForce;

    //Read Only Arrays for the Abilities
    public readonly string[] dodgeAbilities = { "SideStepDodge", "BackPaceDodge" };
    public readonly string[] jumpAbilities = { "RocketJump", "ZeroGravJump", "Jump" };
    public readonly string[] buffAbilities = { "MoveSpeedBuff", "HeightBuff" };

    //player properties
    Rigidbody rb;
    Vector3 move;
    PlayerBase player;
    #endregion

    //Stores the entire string for buffs to display on the BuffUI TMPro Text
    public string BuffUIString;
    public TextMeshProUGUI BuffUI;

    //Checks whether or not the timer for the buff has ended (For the purpose of removing from the UI)
    bool buffTimerReached = true;
    bool dodgeTimerReached;
    //How many times the player may use the super jump (Prevents them from using it repeatedly)
    public float superJumpLimit = 1;
    public float superJumpLimitVal;
    bool hasResetLimit = true;
    //Stores all of the timers to display on the UI
    public List<float> timers = new List<float>();
    //Used to count how many dodges the player has before cooldown
    public int dodgeNumber;
    bool dodgeCooldownActive;

    private void Start()
    {
        //Set references, default limit values and dodge values
        player = PlayerBase.instance;
        rb = GetComponent<Rigidbody>();
        superJumpLimitVal = superJumpLimit;
        dodgeNumber = 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Do not execute when paused, or when viewing a portion of the tutorial
        if (GamePause.paused)
        {
            return;
        }

        if(TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && !TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            return;
        }
        else if (TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            return;
        }

        //Check for inputs for abilities
        checkForAbilityPress();
        //Checks the timers for buffs
        checkForAbilityTimers();
        //counts down the cooldowns for buffs and abilities
        checkForAbilityCooldowns();
        //Checks whether or not the player has touched the ground
        checkForGrounded();
        //Set the buff and ability UI
        SetUI();

    }

    //If the player lands on the ground, reset how many times they can use the rocket jump
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
        //Gets the x direction (For the sake of determining sidestep dodge direction)
        move.x = Input.GetAxisRaw("Horizontal");
    }

    #region Checks
    void checkForAbilityPress()
    {
        //Trigger the corresponding ability for each input (If the countdown is less than 0.)
        //In the dodge's case, it may also trigger if the player still has dodges available
        if (Input.GetButtonDown("Dodge"))
        {
            if(dodgeCooldown <= 0 || dodgeNumber > 0) {
                if (!isDodging)
                {
                    DodgeAbility();
                    dodgeNumber--;
                }
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

    //Checks the timer for the selected buff. If it is greater than 0, count down.
    //Once it reaches 0, reset modifier values to default, and remove the buff from the UI
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
                    speedBuffVal = 1.0f;
                    break;
                case "HeightBuff":
                    player.jumpModifier = 1.0f;
                    jumpBuffVal = 1.0f;
                    break;
            }
            if (!buffTimerReached)
            {
                RemoveBuffFromUI(selectedBuff);
                buffTimerReached = true;
            }
        }
    }

    //Counts down the cooldowns for each ability if they are currently on cooldown
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
        else
        {
            //In the case of the dodge cooldown, once it reaches 0 then reset how many dodges the player may use
            if(dodgeNumber == 0 && dodgeCooldownActive)
            {
                if (selectedDodge.Contains("Side"))
                {
                    dodgeNumber = 2;
                }
                else
                {
                    dodgeNumber = 1;
                }
                dodgeCooldownActive = false;
            }
        }
    }

    #endregion

    #region Dodge Abilities
    void DodgeAbility()
    {
        //Calls the corresponding dodge ability when pressing the input
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
        //Sets the duration of the dodge time
        float dodgeTime = .3f;
        //If the cooldown isn't currently active, set it to cooldown
        if (!dodgeCooldownActive)
        {
            dodgeCooldown = sideStepCooldown;
            maxDodgeCooldown = sideStepCooldown;
            dodgeCooldownActive = true;
        }

        //Set xMove to 1 by default if the player isn't moving (Gives a default dodge direction)
        if (xMove == 0)
        {
            xMove = 1;
        }

        //Set is dodging to true (Prevents movement while dodging)
        isDodging = true;
        //Set velocity to the player's current velocity
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);

        while(dodgeTime > 0)
        {
            //Apply the force to the player to make them dodge while the timer is greater than 0
            rb.AddForce((transform.right * xMove * sideStepDistance) * speedBuffVal, ForceMode.Impulse);
            dodgeTime -= GamePause.deltaTime;
            yield return null;
        }

        
        

        //
        isDodging = false;
        
    }

    //Backpace dodge, functions on a similar principle to the sidestep dodge, but applies it in the opposite of the player's current forward direction
    IEnumerator BackPaceDodgeAbility()
    {
        float dodgeTime = .5f;
        if (!dodgeCooldownActive)
        {
            dodgeCooldown = backPaceCooldown;
            maxDodgeCooldown = backPaceCooldown;
            dodgeCooldownActive = true;
        }
        isDodging = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //rb.AddForce((-transform.forward * backPaceDistance) * speedBuffVal, ForceMode.Impulse);
        while (dodgeTime > 0)
        {
            rb.AddForce(new Vector3(0f, .1f, -transform.forward.z * backPaceDistance) * speedBuffVal, ForceMode.Impulse);
            dodgeTime -= GamePause.deltaTime;
            yield return null;
        }
        
        rb.velocity = Vector3.zero;
        isDodging = false;
        
    }
    #endregion

    #region Buff Abilities
    void BuffAbility()
    {
        //Adds the selected buff to the buff UI
        AddBuffToUI(selectedBuff);
        
        switch (selectedBuff)
        {
            //Sets the specific modifier based on what buff ability is currently selected
            case "MoveSpeedBuff":
                player.speedModifier = speedBuffMod;
                speedBuffVal = speedBuffMod;
                buffTimer = speedBuffTimer;
                buffCooldown = speedBuffCooldown;
                maxBuffCooldown = speedBuffCooldown;
                timers.Add(buffTimer);
                break;
            case "HeightBuff":
                player.jumpModifier = jumpBuffMod;
                jumpBuffVal = jumpBuffMod;
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
            //Rocket Jump: An extremely high jump with vastly reduced movement speed
            case "RocketJump":
                rb.velocity = Vector3.up * player.jumpForce * rocketJumpHeight * jumpBuffVal;
                player.speed = player.speed / 4;
                Invoke("ResetSpeed", 2.2f);
                break;
            //Zero Grav Jump: A jump that provides low gravity, but a reduced jump height
            case "ZeroGravJump":
                rb.useGravity = false;
                defaultJumpForce = player.jumpForce;
                player.jumpForce = player.jumpForce / 1.3f;
                Invoke("ResetGravity", ZeroGravityDuration);
                break;
            //Gives the player an extra normal jump to use in the air
            case "Jump":
                rb.velocity = Vector3.up * player.jumpForce * jumpBuffVal;
                break;
            default:
                //No jump active, do nothing
                break;
        }

        
    }

    //Resets gravity to normal once the zero grav jump ends
    void ResetGravity()
    {
        rb.useGravity = true;
        player.jumpForce = defaultJumpForce;
    }

    //Resets speed to normal after the rocket jump ends
    void ResetSpeed()
    {
        player.speed = player.defaultSpeed;
    }
    #endregion

    //Sets the ability based on what is selected in the simple class UI
    #region Ability Changes
    public void setAbility(string abilityToSet)
    {
        //Checks if it contains a specific keyword (Jump, dodge, Buff)
        if (abilityToSet.Contains("Jump")){
            selectedJump = abilityToSet;
        } else if (abilityToSet.Contains("Dodge"))
        {
           
            selectedDodge = abilityToSet;

            //In the case of the dodge, check for a keyword relating to the "type" (Side, Back)
            if (abilityToSet.Contains("Side"))
            {
                dodgeNumber = 2;
            }
            else if(abilityToSet.Contains("Back"))
            {
                dodgeNumber = 1;
            }
        } else if (abilityToSet.Contains("Buff"))
        {
            selectedBuff = abilityToSet;
        }

        PlayerUI.instance.UpdateGameplayAbilityUI();
    }
    #endregion

    //Sets the buff UI to display currently active buffs and their timers
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

    //Simply adds the name of the buff to the UI
    public void AddBuffToUI(string stringToAdd)
    {
        
        BuffUIString += $"{stringToAdd},";
    }

    public void RemoveBuffFromUI(string stringToRemove)
    {
        //Splits the buff UI string into a string array, and pass these into a list
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

        //If the string is found, then remove it, then shift entries up the array
        for(int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i] == stringToRemove)
            {
                buffs.Remove(stringToRemove);

                buffs = ShiftBuffNames(buffs.ToArray<string>()).ToList<string>();
            }
            
        }

        //Get the string back from the array, by first converting it to an array from a list, then passing it into getstringfromarray
        
        BuffUIString = GetStringFromArray(buffs.ToArray<string>());
    }

    //Initialise a new string variable, join the array into a singular string separated by commas if the array has entries, otherwise set it to blank
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

    //Shifts the names of buffs up the list once one is removed
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
