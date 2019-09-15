using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationScript : MonoBehaviour
{

    public float cameraSensitivity = 1.0f;
    
    public Rigidbody rb;

    public bool firstPersonStandard = false;

    private float pitch, yaw;

    private Quaternion currRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
	currRotation = Quaternion.identity;
    }

    void Update()
    {
	// A normal form of movement
	if (firstPersonStandard) {
	    pitch -= cameraSensitivity * Input.GetAxis("Mouse Y");
	    yaw += cameraSensitivity * Input.GetAxis("Mouse X");
	    this.transform.eulerAngles = new Vector3(pitch, yaw, 0);
	}
	// A form of movement that avoids gimbal lock
	else {
	    Quaternion addRotation = Quaternion.identity;
	    pitch = Input.GetAxis("Mouse Y");
	    yaw = Input.GetAxis("Mouse X");
	    addRotation.eulerAngles = new Vector3(-pitch, yaw, 0);
	    currRotation *= addRotation;
	    rb.MoveRotation(currRotation);
	}

	// Lock cursor if the screen is pressed, unlock if esc is pressed
        if(Input.GetButton("Fire1")) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if(Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
