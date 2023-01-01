using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcentratedShotBullet : BulletObject
{
    //Stores a reference to the player themselves
    PlayerBase ownerPlayer;
    //Rigidbody reference
    Rigidbody rb;
    //float concentratedShotModifier;
    //How fast the shot travels
    float shotSpeed;

    private void Start()
    {
        //Set ownerPlayer to the player instance
        ownerPlayer = PlayerBase.instance;
        damageEnemy = true;
        //Set rb to it' own rigidbody
        rb = GetComponent<Rigidbody>();
        
    }

    //Sets damage based on the following paramaters - the base damage passed in, and the current damage modifier passed in
    public new void setDamage(int bulletDamage, float concentratedModifier)
    {
        //Set the damage to be equal to the damage value param multiplied by the shotModifier
        damage = bulletDamage * concentratedModifier;
    }

    //Speed is set by default
    public void setSpeed(float speed)
    {
        rb.velocity = transform.forward * shotSpeed;
    }

    /*
    public override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            AdvancedEnemyAI targetEnemy = other.gameObject.GetComponent<AdvancedEnemyAI>();
            //Damage enemy (In the advanced enemy AI script, have the checks to determine whether or not the enemy has armour that can be damaged, or to damage them directly
            targetEnemy.TakeDamage(damage, true);

            //Increase damage on hit in player script
            ownerPlayer.GetComponent<DynamicClassAbilities>().increaseConcentratedDamage();
            ownerPlayer.GetComponent<DynamicClassAbilities>().increaseMissileBuffStack();

        }

        Destroy(gameObject);
    }*/

    //When the triggerArea interacts with an enemy (More effective in comparison to the original method of having the object collide, prevents force from being applied)
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            
            //Set targetEnemy to the object it interacts with
            AdvancedEnemyAI targetEnemy = other.gameObject.GetComponent<AdvancedEnemyAI>();
            //Damage enemy (In the advanced enemy AI script, have the checks to determine whether or not the enemy has armour that can be damaged, or to damage them directly
            targetEnemy.TakeDamage(damage, true);

            //Increase damage on hit in player script
            ownerPlayer.GetComponent<DynamicClassAbilities>().increaseConcentratedDamage();
            ownerPlayer.GetComponent<DynamicClassAbilities>().increaseMissileBuffStack();

            //Destroy the bullet
            Destroy(gameObject);

        }

        //Simply destroy the bullet when colliding with the ground
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Destroy the bullet
            Destroy(gameObject);
        }
    }
}
