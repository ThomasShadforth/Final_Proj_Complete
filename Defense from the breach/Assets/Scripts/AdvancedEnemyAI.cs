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
    enemyAIStates defaultState;
    [Header("Enemy State")]
    public enemyAIStates state;

    [Header("Radius - Attack and Detect")]
    public float detectRadius;
    public float attackRadius;
    public float AoEAttackRadius;

    [Header("Speed Values")]
    public float attackSpeed;
    public float moveSpeed;
    public float patrolSpeed;
    public float strafeSpeed;

    [Header("Attack Rates")]
    public float rangeAttackTimer;
    public float meleeAttackTimer;
    float rangeAttackTime, meleeAttackTime;

    [Header("Strafe Values")]
    public float strafeTimer;
    float strafeTime;
    [SerializeField]
    bool canStrafe;
    Transform strafePosition;
    [SerializeField]
    int strafeDirectionMultiplier;

    Transform playerTarget;

    [Header("Patrol Values")]
    public Transform[] patrolPoints;
    public float minPatrolDistance;

    [Header("Enemy Stats")]
    public float enemyMaxHealth;
    public float enemyHealth;
    public bool hasArmour;
    public float ArmourDurability;
    public float defaultArmDurability;
    public bool bossScale;

    [Header("Wait Timers")]
    public float waitTimer;
    public float patrolWaitTimer;
    public float chaseTimer;
    [SerializeField] float waitTime;
    [SerializeField] float attackTimer, attackTime;
    float chaseTime;

    float patrolWaitTime;
    int patrolIndex = 1;

    [Header("Attack Objects")]
    public GameObject bullet;
    public GameObject AOE_Sphere;

    [Header("Misc Objects")]
    public GameObject UpgradePointUI;
    public GameObject DamageUI;
    public GameObject HealthPickup;
    public GameObject AmmoPickup;


    Color AOEMaterialColor;

    public Transform enemyFirepoint;

    public int upgradePointVal;

    bool hasResetRot;
    bool performingAction;
    bool attacking;

    public LayerMask playerLayer;
    public Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
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
        if (GamePause.paused)
        {
            return;
        }

        
        distanceChecks();
        

        //possibility have the timer count down either in the wait state or the strafe state, so the enemy acts out of the strafe instead of just waiting.
        if(state == enemyAIStates.wait)
        {

            if(waitTime > 0)
            {
                waitTime -= GamePause.deltaTime;
                AttackRadiusChecks();
            }
            else
            {
                makeDecision();
            }
        }

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
                state = enemyAIStates.wait;
                performingAction = false;
                waitTime = waitTimer;
                strafeTime = strafeTimer;
                
            }
            
        } else if(state == enemyAIStates.patrol)
        {
            patrol();
        } else if(state == enemyAIStates.chase)
        {
            if(chaseTime > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
                chaseTime -= 1 * GamePause.deltaTime;
                AttackRadiusChecks();
            }
            else
            {
                state = enemyAIStates.wait;
                performingAction = false;
                waitTime = waitTimer;
                chaseTime = chaseTimer;
            }
        }

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
                state = enemyAIStates.wait;
                hasResetRot = false;
                waitTime = waitTimer;
                transform.LookAt(playerTarget.transform);
            }
        }
        else
        {
            state = defaultState;
            performingAction = false;
            if(!hasResetRot && state == enemyAIStates.patrol)
            {
                transform.LookAt(patrolPoints[patrolIndex]);
                hasResetRot = true;
            }
        }
    }

    public void AttackRadiusChecks()
    {
        if(Vector3.Distance(transform.position, playerTarget.position) <= attackRadius)
        {
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
                attackTime = attackTimer;
            }
            else
            {
                attackTime -= 1 * GamePause.deltaTime;
            }

            
        }
    }

    //Will be used to determine AI's next action
    public void makeDecision()
    {
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
            approachPlayer();
        }
    }

    public void approachPlayer()
    {
        state = enemyAIStates.chase;
        performingAction = true;
    }

    
    //This may possibly be divided into different range attacks
    public void rangeAttack()
    {
        GameObject newBullet = Instantiate(bullet, enemyFirepoint.position, enemyFirepoint.rotation);

        if (bossScale)
        {
            Vector3 scalar = transform.localScale;
            scalar *= 1.5f;
            newBullet.transform.localScale = scalar;
        }

        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
        newBullet.GetComponent<BulletObject>().SetOwner(this.gameObject);
        
    }

    public void AoEAttack()
    {
        StartCoroutine(AoECo());
    }

    public void enemyStrafe()
    {
        transform.LookAt(strafePosition);
        transform.RotateAround(strafePosition.position, new Vector3(0, 1, 0), strafeSpeed * strafeDirectionMultiplier * GamePause.deltaTime);
    }

    public void patrol()
    {
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
        else
        {
            patrolWaitTime -= 1 * GamePause.deltaTime;
        }
    }


    public void TakeDamage(float DamageTaken, bool concentratedShot)
    {
       

        if (concentratedShot)
        {
            
            if (hasArmour)
            {
                
                //If it has armour, check if they have armour left
                if (ArmourDurability > 0)
                {
                    ArmourDurability -= DamageTaken;
                }
                else
                {
                    DamageTaken = DamageTaken / 2;
                    enemyHealth -= DamageTaken;
                }
            }
            else
            {
                DamageTaken = DamageTaken / 2;
                enemyHealth -= DamageTaken;
            }
        }
        else
        {
            if (hasArmour)
            {
                if(ArmourDurability > 0)
                {
                    DamageTaken = 1;
                    enemyHealth -= DamageTaken;
                }
                else
                {
                    enemyHealth -= DamageTaken;
                }
            }
            else
            {
                enemyHealth -= DamageTaken;
            }
        }

        GameObject damageUI = Instantiate(DamageUI);
        damageUI.transform.position = transform.position;
        damageUI.transform.rotation = transform.rotation;
        damageUI.GetComponent<DamageIndicator>().setDamage(DamageTaken);

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



    IEnumerator AoECo()
    {
        //Return it to the default colour values
        AOE_Sphere.GetComponent<MeshRenderer>().material.color = AOEMaterialColor;
        AOE_Sphere.SetActive(true);
        Color sphereMaterialColor = AOE_Sphere.GetComponent<MeshRenderer>().material.color;

        while(sphereMaterialColor.a < .8f)
        {
            sphereMaterialColor.a = Mathf.MoveTowards(sphereMaterialColor.a, .8f, 1 * GamePause.deltaTime);
            AOE_Sphere.GetComponent<MeshRenderer>().material.color = sphereMaterialColor;
        }

        yield return new WaitForSeconds(1f);

        Collider[] player = Physics.OverlapSphere(transform.position, AoEAttackRadius, playerLayer);

        if (player.Length > 0)
        {
            if (player[0].GetComponent<PlayerBase>())
            {
                Debug.Log("KNOCKBACK!");
                Vector3 knockDirection = player[0].transform.position - transform.position;
                knockDirection = knockDirection.normalized;
                PlayerBase.instance.hurtPlayer(20);
                PlayerBase.instance.KnockBack(knockDirection);
            }
        }

        AOE_Sphere.SetActive(false);
    }
}
