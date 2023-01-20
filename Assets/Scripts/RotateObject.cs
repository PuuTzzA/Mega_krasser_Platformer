using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    [SerializeField]
    float rotationSpeed = 10f;


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * rotationSpeed);
    }
}