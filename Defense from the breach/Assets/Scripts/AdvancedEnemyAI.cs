using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyAIStates
{
    idle,
    patrol,
    wait,
    strafe,
    chase,
    attack
}

public class AdvancedEnemyAI : MonoBehaviour
{
    //Records the default state for the AI (For when it leaves the chase state
    enemyAIStates defaultState;
    [Header("Enemy State")]
    //Stores the current state of the AI
    public enemyAIStates state;

    [Header("Radius - Attack and Detect")]
    //Radius for detecting the player
    public float detectRadius;
    //Radius in which the enemy will begin to attack the player
    public float attackRadius;
    //Radius for the enemy's AoE (Area of Effect) attack
    public float AoEAttackRadius;

    //Stores the AI's move speed, patrol speed, strafe speed
    [Header("Speed Values")]
    public float moveSpeed;
    public float patrolSpeed;
    public float strafeSpeed;

    
    //Stores the values for strafing (how long the AI strafes, whether they can strafe, the point around which they strafe, and the direction (Which is random))
    [Header("Strafe Values")]
    public float strafeTimer;
    float strafeTime;
    [SerializeField]
    bool canStrafe;
    Transform strafePosition;
    [SerializeField]
    int strafeDirectionMultiplier;

    //The playerTarget, which is automatically set
    Transform playerTarget;

    //Stores data relating to the patrol points, in addition to the minimum distance required to "Complete" a patrol
    [Header("Patrol Values")]
    public Transform[] patrolPoints;
    public float minPatrolDistance;

    //Basic Enemy Information (Health, armour, whether it scales certain attacks for boss size, etc.)
    [Header("Enemy Stats")]
    public float enemyMaxHealth;
    public float enemyHealth;
    public bool hasArmour;
    public float ArmourDurability;
    public float defaultArmDurability;
    public bool bossScale;

    //Stores the default timer values for waiting, chasing, attacking, etc. (As well as creating private variables used to count down)
    [Header("Wait Timers")]
    public float waitTimer;
    public float patrolWaitTimer;
    public float chaseTimer;
    [SerializeField] float waitTime;
    [SerializeField] float attackTimer, attackTime;
    float chaseTime;

    float patrolWaitTime;

    //Stores the current index for the patrol positions (Which one the AI will move to)
    int patrolIndex = 1;

    //Object references for the bullet and AoE_Sphere effect for attacks
    [Header("Attack Objects")]
    public GameObject bullet;
    public GameObject AOE_Sphere;

    //Stores additional object references (Display upgrade points dropped on death, damage values, and a health and ammo pickup object
    [Header("Misc Objects")]
    public GameObject UpgradePointUI;
    public GameObject DamageUI;
    public GameObject HealthPickup;
    public GameObject AmmoPickup;

    //Color of the AoE effect object
    Color AOEMaterialColor;

    //Point from which projectiles are fired
    public Transform enemyFirepoint;

    //How many points are dropped on death
    public int upgradePointVal;

    //Whether or not rotation towards a patrol point has been reset (After chasing the player)
    bool hasResetRot;
    //Whether or not the AI is performing an action or attacking
    bool performingAction;
    bool attacking;

    //Used for detecting the layer on which the player object sits, for AoE attack detectiuon
    public LayerMask playerLayer;
    public Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        //Set default values (State, target, health, etc.)
        defaultState = state;
        playerTarget = PlayerBase.instance.transform;
        enemyHealth = enemyMaxHealth;
        attackTime = attackTimer;
        strafeTime = strafeTimer;
        waitTime = waitTimer;
        chaseTime = chaseTimer;
        patrolWaitTime = patrolWaitTimer;
        originalPosition = transform.position;
        ArmourDurability = defaultArmDurability;
        AOEMaterialColor = AOE_Sphere.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, none of the code will run (Prevents movement while paused)
        if (GamePause.paused)
        {
            return;
        }

        //Runs checks for distance to the playertarget
        distanceChecks();
        

        //possibility have the timer count down either in the wait state or the strafe state, so the enemy acts out of the strafe instead of just waiting.
        if(state == enemyAIStates.wait)
        {
            //While the enemy waits, count down
            if(waitTime > 0)
            {
                waitTime -= GamePause.deltaTime;
                //Check if the player is in range for attacks
                AttackRadiusChecks();
            }
            else
            {
                //Once the timer has counted down, have the AI make a decision
                makeDecision();
            }
        }

        //If the AI is strafing, have it perform the strafe action, countdown, and check for attacks
        if(state == enemyAIStates.strafe)
        {
            if(strafeTime > 0)
            {
                enemyStrafe();
                AttackRadiusChecks();
                strafeTime -= 1 * GamePause.deltaTime;
            }
            else
            {
                //Once strafing ends, reset to wait state
                state = enemyAIStates.wait;
                //Set performing action to false, and reset waitTime and strafeTime
                performingAction = false;
                waitTime = waitTimer;
                strafeTime = strafeTimer;
                
            }
            
        } else if(state == enemyAIStates.patrol)
        {
            //Simply patrol
            patrol();
        } else if(state == enemyAIStates.chase)
        {
            //Briefly chase the player if the timer is greater than 0
            if(chaseTime > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
                chaseTime -= 1 * GamePause.deltaTime;
                //Check for attacks
                AttackRadiusChecks();
            }
            else
            {
                //Once finished chasing, reset states and timers
                state = enemyAIStates.wait;
                performingAction = false;
                waitTime = waitTimer;
                chaseTime = chaseTimer;
            }
        }

        //Set the AI to look at the player if they are currently in range (Ensures rotations are correct for spawning projectiles
        if(Vector3.Distance(playerTarget.position, transform.position) < detectRadius)
        {
            transform.LookAt(playerTarget.transform);
            enemyFirepoint.transform.LookAt(playerTarget.transform);
        }
    }

    public void distanceChecks()
    {
        //if player in range then chase...
        //in normal context
        //For advanced AI, have the enemy turn to face the player
        //then have decisions made at random
        if(Vector3.Distance(transform.position, playerTarget.position) <= detectRadius)
        {
            
            //For now, have enemy immediately respond to player's presence
            //Can always tweak to respond to sounds/attacks later
            if(state != enemyAIStates.wait && !performingAction && !attacking)
            {
                //Wait is used as a default for being in range. When in the wait state, the enemy will act after a short period of time
                state = enemyAIStates.wait;
                //Rotation reset bool for patrol points is reset
                hasResetRot = false;
                //Set wait time
                waitTime = waitTimer;

                //Look at the player
                transform.LookAt(playerTarget.transform);
            }
        }
        else
        {
            //When the player is no longer in range, reset to the default state, stop performing actions, and if the default state was patrol, look at the patrol point
            state = defaultState;
            performingAction = false;
            if(!hasResetRot && state == enemyAIStates.patrol)
            {
                transform.LookAt(patrolPoints[patrolIndex]);
                hasResetRot = true;
            }
        }
    }

    //Used to check if the player is within range for attacks
    public void AttackRadiusChecks()
    {
        //If the player is in range, start attacking
        if(Vector3.Distance(transform.position, playerTarget.position) <= attackRadius)
        {
            //When the timer has counted down to 0, check if the player is in range for an AoE. if they are, use an AoE. Otherwise, fire a projectile
            if(attackTime <= 0)
            {
                if (Vector3.Distance(transform.position, playerTarget.position) > AoEAttackRadius)
                {
                    rangeAttack();
                    
                }
                else
                {
                    AoEAttack();
                    
                }
                //Reset the timer
                attackTime = attackTimer;
            }
            else
            {
                //Count down
                attackTime -= 1 * GamePause.deltaTime;
            }

            
        }
    }

    //Will be used to determine AI's next action
    public void makeDecision()
    {
        //Gewnerate a random int. If the int is 0, then the AI will begin to strafe in a random direction based on another random value.
        int randomNum = Random.Range(0, 2);
        if(randomNum == 0)
        {
            
            strafePosition = playerTarget;
            var randomVal = Random.Range(0.50f, 1.00f);
            state = enemyAIStates.strafe;
            performingAction = true;
            if(randomVal >= .75f)
            {
                strafeDirectionMultiplier = 1;
            }
            else
            {
                strafeDirectionMultiplier = -1;
            }
        }
        else
        {
            //Otherwise, the AI will attempt to approach the player in the chase state
            approachPlayer();
        }
    }

    public void approachPlayer()
    {
        //Set the chase state, and performing action to true
        state = enemyAIStates.chase;
        performingAction = true;
    }

    
    
    public void rangeAttack()
    {
        //Set velocity scale to 1f. This is changed based on whether or not bossScale is true
        float velocityScale = 1f;
        //Instantiate the bullet projectile, set the owner (For the purpose of collision/trigger detection)
        GameObject newBullet = Instantiate(bullet, enemyFirepoint.position, enemyFirepoint.rotation);
        newBullet.GetComponent<BulletObject>().SetOwner(this.gameObject, false);
        //If the bosscale is set, then scale the object up
        if (bossScale)
        {
            Vector3 scalar = transform.localScale;
            scalar *= 1.1f;
            newBullet.transform.localScale = scalar;
            newBullet.GetComponent<SphereCollider>().radius = .1f;
            velocityScale = 2f;
        }

        //Set the velocity based on the set velocity scale
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * 10 * velocityScale;
        
        
    }

    public void AoEAttack()
    {
        //Calls the respective AoE coroutine for attacking
        StartCoroutine(AoECo());
    }

    //Strafing action. The AI will move in it's chosen direction while the timer for strafing is greater than 0
    public void enemyStrafe()
    {
        transform.LookAt(strafePosition);
        transform.RotateAround(strafePosition.position, new Vector3(0, 1, 0), strafeSpeed * strafeDirectionMultiplier * GamePause.deltaTime);
    }

    public void patrol()
    {
        //Patrol action. When the AI has finished waiting, begin moving towards the current patrol point
        //If the distance to the point is less than the minimum required distance, then set a new index, look at the next point, and set the wait time
        if(patrolWaitTime <= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[patrolIndex].position, patrolSpeed * GamePause.deltaTime);

            if(Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < minPatrolDistance)
            {
                patrolIndex++;

                if(patrolIndex > patrolPoints.Length - 1)
                {
                    patrolIndex = 0;
                }

                transform.LookAt(patrolPoints[patrolIndex]);
                patrolWaitTime = patrolWaitTimer;
            }
        }
        //Count down the wait timer
        else
        {
            patrolWaitTime -= 1 * GamePause.deltaTime;
        }
    }

    //Takes damage from the player attacks
    public void TakeDamage(float DamageTaken, bool concentratedShot)
    {
       
        //If the projectile that hits them is a concentrated shot bullet, then check if the AI has armour (It should only be truly effective against armour)
        if (concentratedShot)
        {
            
            if (hasArmour)
            {
                
                //If it has armour, check if they have armour left
                //If they do, then damage the armour
                if (ArmourDurability > 0)
                {
                    ArmourDurability -= DamageTaken;
                }
                //Otherwise, halve the damage from the projectile and take the damage
                else
                {
                    DamageTaken = DamageTaken / 2;
                    enemyHealth -= DamageTaken;
                }
            }
            //If the AI has no armour at all (Is not an armoured type), then cut the damage to a 6th of what it would be, then take it away
            else
            {
                DamageTaken = DamageTaken / 6;
                enemyHealth -= DamageTaken;
            }
        }
        else
        {
            //If the projectile was a regular bullet, check for armour
            //If they have armour, then reduce damage to 1
            if (hasArmour)
            {
                if(ArmourDurability > 0)
                {
                    DamageTaken = 1;
                    enemyHealth -= DamageTaken;
                }
                //If no armour remains, take the regular amount of damage
                else
                {
                    enemyHealth -= DamageTaken;
                }
            }
            //If they aren't an armoured enemy, then take regular damage
            else
            {
                enemyHealth -= DamageTaken;
            }
        }

        //Spawn the damage UI on the enemy, set it's position to the current position and rotation, and set the damage text to the amount taken

        GameObject damageUI = Instantiate(DamageUI);
        damageUI.transform.position = transform.position;
        damageUI.transform.rotation = transform.rotation;
        damageUI.GetComponent<DamageIndicator>().setDamage(DamageTaken);

        //If the enemy's health has been reduced to 0, then spawn the UI for dropped points, spawn pickups, and destroy the enemy
        if(enemyHealth <= 0)
        {
            GameObject PointUI = Instantiate(UpgradePointUI);
            PointUI.transform.position = transform.position;
            PointUI.GetComponent<UpgradePointIndicator>().SetUI(upgradePointVal);
            
            SpawnPickups();

            GameManager.instance.upgradePoints += upgradePointVal;
            Destroy(this.gameObject);
        }
    }

    private void SpawnPickups()
    {
        //Call a method that generates a random number between .2f and 1f. If it's greater than or equal to .5f, then spawn a pickup (Applies to both health and ammo)
        if (PickupSpawn() >= .5f)
        {
            GameObject healthPickup = Instantiate(HealthPickup);
            healthPickup.transform.position = transform.position;
            healthPickup.GetComponent<Rigidbody>().AddForce(new Vector3(-3f, 3f, 0f), ForceMode.Impulse);
        }

        if (PickupSpawn() >= .5f)
        {
            GameObject ammoPickup = Instantiate(AmmoPickup);
            ammoPickup.transform.position = transform.position;
            ammoPickup.GetComponent<Rigidbody>().AddForce(new Vector3(3f, 3f, 0), ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }


    public float PickupSpawn()
    {
        return Random.Range(.2f, 1f);
    }


    //CoRoutine for the AoE attack
    IEnumerator AoECo()
    {
        //Return it to the default colour values
        AOE_Sphere.GetComponent<MeshRenderer>().material.color = AOEMaterialColor;
        //Set the sphere to be active
        AOE_Sphere.SetActive(true);
        //Store a reference to the current colour of the effect sphere
        Color sphereMaterialColor = AOE_Sphere.GetComponent<MeshRenderer>().material.color;

        while(sphereMaterialColor.a < .8f)
        {
            //Make the sphere become more opaque over time
            sphereMaterialColor.a = Mathf.MoveTowards(sphereMaterialColor.a, .8f, .4f * GamePause.deltaTime);
            AOE_Sphere.GetComponent<MeshRenderer>().material.color = sphereMaterialColor;
        }

        //When enough time has passed, check for the player's collider
        yield return new WaitForSeconds(1.3f);

        Collider[] player = Physics.OverlapSphere(transform.position, AoEAttackRadius, playerLayer);

        if (player.Length > 0)
        {
            //If the first entry has the player's script attached
            if (player[0].GetComponent<PlayerBase>())
            {
                //Calculate the direction of the player's knockback
                Vector3 knockDirection = player[0].transform.position - transform.position;
                knockDirection = knockDirection.normalized;
                
                //Damage the player, and apply knockback
                PlayerBase.instance.hurtPlayer(20);
                PlayerBase.instance.KnockBack(knockDirection);
            }
        }

        //Set the sphere to be inactive after the attack finishes
        AOE_Sphere.SetActive(false);
    }
}
