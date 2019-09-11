using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationScript : MonoBehaviour
{

    public float cameraSensitivity = 1.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        pitch -= cameraSensitivity * Input.GetAxis("Mouse Y");
	    yaw += cameraSensitivity * Input.GetAxis("Mouse X");
	    this.transform.eulerAngles = new Vector3(pitch, yaw, 0);

        if(Input.GetButton("Fire1")) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if(Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
