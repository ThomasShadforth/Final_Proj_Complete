using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    //Used to control the rotation for the camera (For Firt Person and the weapon)

    //Sensitivty controls the speed at which the camera rotates
    public float sensitivity;
    //xRot stores rotation on the x axis
    float xRot = 0;

    //The 
    [SerializeField] Transform playerChar;
    [SerializeField] Transform firepointOrigin;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.paused)
        {
            return;
        }
        float xMouse = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float yMouse = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= yMouse;

        xRot = Mathf.Clamp(xRot, -90, 90);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerChar.Rotate(Vector3.up * xMouse);

        

        //firepointOrigin.Rotate(vec)
        //firepointOrigin.RotateAround(playerChar.position, new Vector3(xRot, 0f, 0f), 0);
    }
}
