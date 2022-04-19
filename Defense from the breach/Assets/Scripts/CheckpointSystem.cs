using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public Vector3 currentCheckpointPos;

    public static CheckpointSystem instance;

    void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        currentCheckpointPos = PlayerBase.instance.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayerPos()
    {
        PlayerBase.instance.transform.position = currentCheckpointPos;
    }

    public void setCurrentCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpointPos = checkpointPosition;
    }
}
