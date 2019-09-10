using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{

    public float movementSpeed = 100.0f;
    public float sprintFactor = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	float speed = movementSpeed * Time.deltaTime;
	if (Input.GetButton("Fire1")) speed *= sprintFactor;

	this.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed);
	this.transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed);
	this.transform.Translate(Vector3.up * Input.GetAxis("Jump") * speed);
    }
}
