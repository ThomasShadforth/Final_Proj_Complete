using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    [SerializeField]
    float destroyTime;
    public float damage;
    GameObject owner;
    protected bool damageEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetOwner(GameObject ownerToSet, bool willHurtEnemy)
    {
        owner = ownerToSet;
        damageEnemy = willHurtEnemy;
    }

    public virtual void setDamage(int bulDamage, float wepDmgMod)
    {
        damage = bulDamage * wepDmgMod;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;

        if(destroyTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    /*public virtual void (Collision other)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (other.gameObject.GetComponent<EnemyAI>())
        {

            EnemyAI hurtEnemy = other.gameObject.GetComponent<EnemyAI>();
            hurtEnemy.hurtEnemy(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.GetComponent<PlayerBase>() && owner != other.gameObject.GetComponent<PlayerBase>().GetComponent<GameObject>())
        {
            PlayerBase.instance.hurtPlayer(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.GetComponent<AdvancedEnemyAI>() && owner != other.gameObject.GetComponent<AdvancedEnemyAI>().GetComponent<GameObject>())
        {
            AdvancedEnemyAI hurtEnemy = other.gameObject.GetComponent<AdvancedEnemyAI>();
            hurtEnemy.TakeDamage(damage, false);
            Destroy(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }*/

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        
        if (other.gameObject.GetComponent<PlayerBase>() && !damageEnemy)
        {
            PlayerBase.instance.hurtPlayer(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            if (damageEnemy)
            {

                AdvancedEnemyAI hurtEnemy = other.gameObject.GetComponent<AdvancedEnemyAI>();
                hurtEnemy.TakeDamage(damage, false);
                Destroy(gameObject);
            }
        }

        else if(!other.gameObject.GetComponent<BulletObject>())
        {
            Destroy(gameObject);
        }
    }
}
