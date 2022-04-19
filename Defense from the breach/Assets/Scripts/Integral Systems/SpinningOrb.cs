using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningOrb : MonoBehaviour
{
    //Speed and damage values
    public float speed;
    public float damage;

    //Get the animator
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDamage(float modifier)
    {
        damage = damage * modifier;
    }

    private void OnCollisionEnter(Collision other)
    {
        //If the projectile collides with the ground or with and enemy, trigger
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            //If it's an enemy, damage them with the respective damage values
            if (other.gameObject.GetComponent<AdvancedEnemyAI>())
            {
                other.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(damage, false);
            }

            //Destroy the projectile
            Destroy(gameObject);
        }
    }
}
