using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{

    public float movementSpeed = 100.0f;
    public float sprintFactor = 2.0f;

    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    float speed = movementSpeed;
	    if (Input.GetButton("Fire1")) speed *= sprintFactor;

        Vector3 vertical, horizonal, jump;
        vertical = transform.forward * Input.GetAxis("Vertical") * speed;
        horizonal = transform.right * Input.GetAxis("Horizontal") * speed;
        jump = transform.up * Input.GetAxis("Jump") * speed;

        rigidbody.velocity = vertical + horizonal + jump;
    }
}
