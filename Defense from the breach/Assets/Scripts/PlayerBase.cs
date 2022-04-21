using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public enum classType
{
    Simple,
    Dynamic
}

public class PlayerBase : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float defaultSpeed;
    Vector3 moveInput;

    [Header("Ground Checks")]
    [SerializeField]
    Transform playerFeetPos;
    public float detectRadius;
    public LayerMask whatIsGround;
    public bool isGrounded;

    [Header("Jump Variables")]
    public int jumpCount;
    public float jumpForce;
    public float rocketJumpMult;
    float defaultJumpForce;
    public float jumpTimer;
    float jumpTime;
    int jumps;
    bool isJumping;

    Vector3 moveVector;

    [Header("Buff Modifiers")]
    public float speedModifier;
    public float jumpModifier;

    SimpleClassAbilities SimpleAbilities;
    DynamicClassAbilities DynamicAbilities;
    [SerializeField]WeaponTypes weapon;

    [Header("General Stats")]
    public float health;
    public float maxHealth;


    //Singleton Structure
    public static PlayerBase instance;

    [Header("Class System")]
    public classType selectedClass = classType.Simple;

    public float knockbackForce;
    public float knockbackTimer;
    float knockbackTime;

    
    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        defaultSpeed = speed;
        SimpleAbilities = GetComponent<SimpleClassAbilities>();
        DynamicAbilities = GetComponent<DynamicClassAbilities>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GamePause.paused)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(playerFeetPos.position, detectRadius, whatIsGround);

        if (knockbackTime <= 0)
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.z = Input.GetAxis("Vertical");
        }

        

        moveVector = ((transform.forward * moveInput.z * speed) + (transform.right * moveInput.x * speed)) * speedModifier;
        

        
    }

    private void Update()
    {
        if (GamePause.paused)
        {
            return;
        }

        if ((TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && !TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared)) || (TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared)))
        {
            return;
        }
        /*else if (TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            
            return;
        }*/

        if (knockbackTime <= 0)
        {

            checkForJump();
        }
        else
        {
            knockbackTime -= GamePause.deltaTime;
        }

        //PlayerRotation Based on Mouse Movement

        //transform.Rotate(transform.rotation.x, Input.GetAxis("Mouse X"), transform.rotation.z);



        CheckForClassSwitch();


        //transform.position += moveVector;

        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }

    #region Jump

    public void checkForJump()
    {
        //Vertical Movement (Jumps)
        if (Input.GetButtonDown("Jump") && jumps > 0)
        {
            isJumping = true;
            jumpTime = jumpTimer;
            rb.velocity = Vector3.up * jumpForce * jumpModifier;
            jumps--;
        }


        else if (Input.GetButtonDown("Jump") && isGrounded && jumps == 0)
        {
            isJumping = true;
            jumpTime = jumpTimer;
            rb.velocity = Vector3.up * jumpForce * jumpModifier;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if(jumpTime > 0)
            {
                rb.velocity = Vector3.up * jumpForce * jumpModifier;
                jumpTime -= GamePause.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (isGrounded)
        {
            jumps = jumpCount;
        }
        
    }

    #endregion

    

    #region Ability/Class Setting
    public void activateClass(classType selectedClass)
    {
        if(selectedClass == classType.Simple)
        {
            SimpleAbilities.enabled = true;
            DynamicAbilities.enabled = false;
            if(SimpleAbilities.BuffUI == null)
            {
                SimpleAbilities.BuffUI = PlayerUI.instance.simpleBuffText;
            }

            weapon.gameObject.SetActive(false);
            PlayerUI.instance.SetClassGameplayUI();
            PlayerUI.instance.UpdateGameplayAbilityUI();
            speedModifier = 1f;
            jumpModifier = 1f;
        } else if(selectedClass == classType.Dynamic)
        {
            SimpleAbilities.enabled = false;
            DynamicAbilities.enabled = true;
            if (DynamicAbilities.BuffUI == null)
            {
                DynamicAbilities.BuffUI = PlayerUI.instance.dynamicBuffText;
            }
            weapon.gameObject.SetActive(true);
            PlayerUI.instance.SetClassGameplayUI(true);
            PlayerUI.instance.UpdateGameplayAbilityUI(true);
            speedModifier = .4f;
            jumpModifier = .4f;
        }
    }


    public void setAbility(string abilityName)
    {
        if(selectedClass == classType.Simple)
        {
            SimpleAbilities.setAbility(abilityName);
        }
        else
        {
            DynamicAbilities.SetAbility(abilityName);
        }
        
        
    }

    public void CheckForClassSwitch()
    {
        if (Input.GetButtonDown("SwapClass"))
        {
            ChangeClass();
        }
    }

    public void ChangeClass()
    {
        classType[] classes = (classType[])System.Enum.GetValues(typeof(classType));

        foreach(classType type in classes)
        {
            if(type != selectedClass)
            {
                selectedClass = type;
                activateClass(selectedClass);
                break;
            }
        }
    }

    #endregion

    #region Value Changes
    public void hurtPlayer(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            transform.position = CheckpointSystem.instance.currentCheckpointPos;
            transform.position = new Vector3(transform.position.x - 5, transform.position.y, transform.position.z);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            health = maxHealth;
        }
    }

    public void KnockBack(Vector3 direction)
    {
        knockbackTime = knockbackTimer;

        direction.y = .2f;
        direction.z = 2f;

        Debug.Log(direction);
        //rb.AddForce(direction, ForceMode.Impulse);

        StartCoroutine(knockbackCo(direction, knockbackTime));
        Debug.Log(rb.velocity);
        
    }

    IEnumerator knockbackCo(Vector3 direction, float knockTime)
    {
        while(knockTime > 0)
        {
            rb.AddForce(direction, ForceMode.Impulse);
            knockTime -= GamePause.deltaTime;
            yield return null;
        }
    }

    public void AddHealth(float healthRecovery)
    {
        health += healthRecovery;

        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
    

    public void AddAmmo(int ammoGain)
    {
        DynamicAbilities.weapon.AmmoHeld += ammoGain;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(playerFeetPos.position, detectRadius);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SimpleAbilities.enabled)
        {
            SimpleAbilities.BuffUI = PlayerUI.instance.simpleBuffText;
        }

        if (DynamicAbilities.enabled)
        {
            DynamicAbilities.BuffUI = PlayerUI.instance.dynamicBuffText;
        }
    }
    
}
