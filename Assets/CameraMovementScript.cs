using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{

    public float movementSpeed = 100.0f;
    public float sprintFactor = 2.0f;

    public Rigidbody rb;

    void Update()
    {
	// Allow increased movement speed
	float speed = movementSpeed;
	if (Input.GetButton("Fire1")) speed *= sprintFactor;

        Vector3 vertical, horizonal, jump;
        vertical = transform.forward * Input.GetAxis("Vertical") * speed;
        horizonal = transform.right * Input.GetAxis("Horizontal") * speed;

	// Add extra movement for going directly up and down in relation to the world
        jump = Vector3.up * Input.GetAxis("Jump") * speed;

        rb.velocity = vertical + horizonal + jump;
    }
}
