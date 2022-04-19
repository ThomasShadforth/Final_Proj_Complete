using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmightyWaveDetonation : MonoBehaviour
{
    //Floats storing the time it takes for the projectile to finish detonating (Destroy time) and the damage it deals to the enemy
    public float detonationTime;
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, do not execute any of the code
        if (GamePause.paused)
        {
            return;
        }

        if(detonationTime > 0)
        {
            detonationTime -= GamePause.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void setDamage(float damageModifier)
    {
        damage = damage * damageModifier;
    }

    //OnTriggerStay will damage the enemy while they remain within the projectile's trigger area
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            other.GetComponent<AdvancedEnemyAI>().TakeDamage(damage, false);
        }
    }
}
