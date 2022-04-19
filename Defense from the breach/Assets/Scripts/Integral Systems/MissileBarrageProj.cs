using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBarrageProj : MonoBehaviour
{
    //The missile's base damage, the adjustable damage
    public float missileDamageBase;
    public float missileDamage;
    //How long it takes for the missile to lock onto an enemy
    public float lockOnTime;
    //the radius in which enemies have to be in to be targeted
    public float missileCheckRadius;
    //How long the missile remains active before being destroyed
    public float missileLifeTime;
    //The player character
    PlayerBase ownerPlayer;
    //The enemy layer (used to check only that layer)
    public LayerMask enemyLayer;
    //The target that is set after checking in the radius
    [SerializeField] Transform targetEnemy;
    //Bool to check if the target has been set or not
    bool targetSet;

    // Start is called before the first frame update
    void Start()
    {
        //Upon instantiation, set the owner player, and call missileHoming
        ownerPlayer = PlayerBase.instance;
        MissileHoming();
        
    }

    // Update is called once per frame
    void Update()
    {
        //If the target has been set, look at the enemy, then move towards the enemy target
        if (targetSet)
        {
            transform.LookAt(targetEnemy);
            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position,  15 * GamePause.deltaTime);
            //GetComponent<Rigidbody>().MovePosition(targetEnemy.position);
        }

        if(missileLifeTime > 0)
        {
            missileLifeTime -= GamePause.deltaTime;

        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    //Set the damage upon being instantiated in the dynamic ability script
    public void SetDamage(float damageModifier)
    {
        missileDamage = (missileDamageBase * damageModifier);
    }

    //Invoked in the missileHoming method
    public void SetTarget()
    {
        //Initialise a collider array
        Collider[] targetEnem = Physics.OverlapSphere(transform.position, missileCheckRadius, enemyLayer);

        //If the length is 0, simply ignore the rest of the code
        if(targetEnem.Length == 0)
        {
            return;
        }
        else
        {
            //Set the target to the first entry of the array
            targetEnemy = targetEnem[0].gameObject.transform;
            //Set this objects rigidbody to 0 (Prevents it from moving up while moving down
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //targetSet is set to true
            targetSet = true;
        }

        
    }

    //Invokes setTarget with the lock on time
    public void MissileHoming()
    {
        Invoke("SetTarget", lockOnTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            //Damage the enemy, and increase the projectile buff modifier
            other.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(missileDamage, false);
            ownerPlayer.GetComponent<DynamicClassAbilities>().increaseProjectilesModifier();
        }

        //Destroy the object

        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, missileCheckRadius);
    }
}
