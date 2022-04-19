using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmightyWaveBase : MonoBehaviour
{
    //set the projectile speed, the damage it deals on impact and the detonation object (The lingering effect it leaves behind)
    public float projectileSpeed;
    public float baseDamage;
    public float damage;
    float projectileModifier;
    public GameObject detonationObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDamage(float modifier)
    {
        damage = baseDamage * modifier;
        projectileModifier = modifier;
    }

    private void OnCollisionEnter(Collision other)
    {
        //If it collides with either an enemy or the ground , destroy the object and instantiate the lingering effect
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            GameObject detonation = Instantiate(detonationObject, transform.position, transform.rotation);
            detonation.GetComponent<AlmightyWaveDetonation>().setDamage(projectileModifier);
            //If it's the enemy, have the enemy receive damage from this
            if (other.gameObject.GetComponent<AdvancedEnemyAI>())
            {
                other.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(damage, false);
            }
            Destroy(this.gameObject);
        }
    }
}
