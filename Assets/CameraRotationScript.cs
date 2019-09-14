using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationScript : MonoBehaviour
{

    public float cameraSensitivity = 1.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
	// Control mouse movement
	pitch -= cameraSensitivity * Input.GetAxis("Mouse Y");
	yaw += cameraSensitivity * Input.GetAxis("Mouse X");
	this.transform.eulerAngles = new Vector3(pitch, yaw, 0);

	// Lock cursor if the screen is pressed, unlock if esc is pressed
        if(Input.GetButton("Fire1")) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if(Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
