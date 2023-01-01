using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    //Script attached to the camera that controls the rotation for the First Person Camera (And the weapon's rotation by extension)

    //Stores the current xRotation
    public float xRot = 0;

    //Sensitivity affects how quickly the camera can move
    public float Sensitivity;
    //stores the player's transform. rotates their body based on the mouse's x movement
    public Transform playerBody;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //xMouse is equal to the movement on the Mouse's x axis multiplied by sensitivty and game time
        float xMouse = Input.GetAxis("Mouse X") * Sensitivity * GamePause.deltaTime;
        //yMouse is equal to the movement on the Mouse's y axis multiplied by sensitivty and game time
        float yMouse = Input.GetAxis("Mouse Y") * Sensitivity * GamePause.deltaTime;

        //Set the xRotation  to yMouse (as rotation on the xAxis is vertical)
        xRot -= yMouse;
        //Clamp the value between -90 and 90 (Wholly up and wholly down)
        xRot = Mathf.Clamp(xRot, -90, 90);


        //Set the camera's rotation to be that on the x axis based on xRot
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        //Rotate the player's body based on the xMouse value.
        playerBody.Rotate(Vector3.up * xMouse);
    }
}
