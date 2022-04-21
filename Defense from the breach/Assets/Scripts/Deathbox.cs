using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deathbox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerBase>())
        {
            PlayerBase.instance.transform.position = CheckpointSystem.instance.currentCheckpointPos;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
