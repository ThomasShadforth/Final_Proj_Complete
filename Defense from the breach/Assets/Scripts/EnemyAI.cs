using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum enemyStates
{
    idle,
    patrol,
    chase,
    attack
}
public class EnemyAI : MonoBehaviour
{
    enemyStates defaultState;
    [Header("Enemy State")]
    public enemyStates state;

    [Header("Chase Values")]
    public float chaseRadius;
    public float attackRadius;

    public float moveSpeed, chaseSpeed, patrolSpeed;
    public float rateOfAttack;
    float attackTimer;

    public float enemyMaxHealth;
    public float enemyHealth;

    [Header("Player Target Values")]
    [SerializeField]
    Transform target;

    [Header("Patrol Values")]
    [SerializeField]
    Transform[] patrolPoints;
    public float minimumCheckpointRadius;
    public float waitTime;
    float waitTimer;
    int patrolIndex = 1;

    [Header("Miscellaneous")]
    [SerializeField]
    GameObject bullet;

    [SerializeField]
    Transform enemyFirepoint;

    bool hasResetRot;
    Vector3 originalPosition;

    public int upgradePointVal;

    public bool isStrafing, canStrafe;

    void Start()
    {
        defaultState = state;
        target = PlayerBase.instance.transform;
        originalPosition = transform.position;
        enemyHealth = enemyMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.paused)
        {
            return;
        }

        //Distance Checks

        distanceChecks();
        
        
        if(state == enemyStates.chase || state == enemyStates.attack)
        {
            //Chase the player (Separate Method)
            chaseTarget();
        } else if(state == enemyStates.idle)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * GamePause.deltaTime);
        } else if(state == enemyStates.patrol)
        {
            patrol();
        }

        if(attackTimer > 0)
        {
            attackTimer -= 1 * GamePause.deltaTime;
        }
    }

    public void chaseTarget()
    {
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * GamePause.deltaTime);

        if (Vector3.Distance(target.position, transform.position) < attackRadius)
        {
            state = enemyStates.attack;
        }

        if (state == enemyStates.attack)
        {
            if (attackTimer <= 0)
            {
                GameObject newBullet = Instantiate(bullet, enemyFirepoint.position, enemyFirepoint.rotation);
                newBullet.GetComponent<Rigidbody>().velocity = transform.forward * 10;
                attackTimer = rateOfAttack;
            }
        }
    }

    void distanceChecks()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius)
        {
            //Change to chase state here
            state = enemyStates.chase;
            hasResetRot = false;
        }
        else
        {
            //return to idle/patrol
            state = defaultState;
            //if idle, return to default position
            if (!hasResetRot && state == enemyStates.patrol)
            {
                transform.LookAt(patrolPoints[patrolIndex]);
                hasResetRot = true;
            }
            
        }
    }

    void patrol()
    {
        if (waitTimer <= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[patrolIndex].position, moveSpeed * GamePause.deltaTime);
            if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) <= minimumCheckpointRadius)
            {

                //Move on to the next patrol point
                patrolIndex++;
                
                if (patrolIndex >= patrolPoints.Length)
                {
                    patrolIndex = 0;
                }
                transform.LookAt(patrolPoints[patrolIndex]);
                waitTimer = waitTime;
            }
        }
        else
        {
            waitTimer -= 1 * GamePause.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    public void hurtEnemy(float damageDealt)
    {
        enemyHealth -= damageDealt;
        if(enemyHealth <= 0)
        {
            GameManager.instance.upgradePoints += upgradePointVal;
            Destroy(gameObject);
        }
    }


    public void approachDecision()
    {

    }
}
