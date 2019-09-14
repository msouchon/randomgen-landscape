using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour
{

    public float speed = 10;

    void Update()
    {
        transform.Rotate(Vector3.back * (speed * Time.deltaTime));
    }
}
