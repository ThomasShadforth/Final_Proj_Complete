using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterFragPiece : MonoBehaviour
{
    //The rigidbody of the object
    Rigidbody rb;
    //Damage of fragpieces
    public float fragPieceDamage = 10f;
    //The move speed and the height of the frag piece
    public float fragMoveSpeed, fragHeight;
    public float radius;
    public LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        //Get the attached rigidbody
        rb = GetComponent<Rigidbody>();
        //remove the transform parent (Prevents it from being destroyed)
        transform.parent = null;
        //Set the velocity
        rb.velocity = transform.forward * fragMoveSpeed + transform.up * fragHeight;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        /*
        //Damage the enemy on hit
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            if (other.gameObject.GetComponent<AdvancedEnemyAI>())
            {
                other.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(fragPieceDamage, false);
            }
            
            
            Destroy(gameObject);
            
        }*/

        Collider[] detectedEnemies = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        if(detectedEnemies.Length != 0)
        {
            foreach(Collider enemy in detectedEnemies)
            {
                enemy.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(fragPieceDamage, false);
            }
        }

        Destroy(gameObject);
    }
}
