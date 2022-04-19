using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform PlayerTransform;
    float YOffset;
    private void Start()
    {
        PlayerTransform = PlayerBase.instance.transform;
        YOffset = transform.position.y - PlayerTransform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y + YOffset, transform.position.z);
    }

    // Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 minimapPosition = PlayerTransform.position;
        minimapPosition.y = transform.position.y;

        transform.position = minimapPosition;
        
    }
}
