using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{

    public float movementSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	this.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * movementSpeed);
	this.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * movementSpeed);
    }
}
