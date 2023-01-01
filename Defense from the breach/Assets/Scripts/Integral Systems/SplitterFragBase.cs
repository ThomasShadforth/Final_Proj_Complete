using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterFragBase : MonoBehaviour
{
    //The rigidbody of the object
    Rigidbody rb;
    //the detonate time (How long time has to pass before detonating)
    public float detonateTime;
    //Reference to the frag piece prefab
    [SerializeField] GameObject fragPiecePrefab;
    //Array that stores references to all the positions where frag pieces can spawn
    [SerializeField] Transform[] fragPieceSpawns;
    //The baseDamage of the projectile
    public float baseDamage;
    //The damage of the fragPieces
    public float fragPieceDamage;
    //Whether or not the object has spawned frag pieces
    bool hasSpawnedFrags;
    public float damageRadius;
    public LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If the detonation time is greater than 0, count down in game time
        if(detonateTime > 0)
        {
            detonateTime -= GamePause.deltaTime;
        }
        else
        {
            //If the frags haven't been spawned, then start the coroutine and set the boolean to true
            if (!hasSpawnedFrags)
            {
                StartCoroutine(SpawnFragPieces());
                hasSpawnedFrags = true;
            }
        }
    }

    //On collision, if the other object is the ground or an an enemy, trigger this block of code
    private void OnCollisionEnter(Collision other)
    {
        /*
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.GetComponent<AdvancedEnemyAI>())
        {
            //If it's an enemy, have them take damage
            if (other.gameObject.GetComponent<AdvancedEnemyAI>())
            {
                other.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(baseDamage, false);
            }

            //Start the frag pieces coroutine
            if (!hasSpawnedFrags)
            {
                StartCoroutine(SpawnFragPieces());
                hasSpawnedFrags = true;
            }
        }*/

        Collider[] detectedEnemies = Physics.OverlapSphere(transform.position, damageRadius, enemyLayer);
        if(detectedEnemies.Length != 0)
        {
            foreach(Collider enemy in detectedEnemies)
            {
                enemy.gameObject.GetComponent<AdvancedEnemyAI>().TakeDamage(baseDamage, false);
            }
        }

        if (!hasSpawnedFrags)
        {
            StartCoroutine(SpawnFragPieces());
            hasSpawnedFrags = true;
        }
    }

    IEnumerator SpawnFragPieces()
    {
        //For each of the spawn positions, first initialise an int variable called fragLimit (Used to limit how many frags will spawn)
        for(int i = 0; i < fragPieceSpawns.Length; i++)
        {
            int fragLimit = 0;

            //If it's not 1, instantiate the fragPiece, set it to 1.
            if (fragLimit != 1)
            {
                GameObject fragPiece = Instantiate(fragPiecePrefab, fragPieceSpawns[i].position, fragPieceSpawns[i].rotation);
                fragLimit = 1;
            }
            //Start the loop again with no delay

            yield return null;
        }

        //Destroy the object

        Destroy(this.gameObject);
    }
}
