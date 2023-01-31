using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnowballLaunch : MonoBehaviour
{
    public Transform throwPoint;
    public GameObject snowball;

    [SerializeField]
    private float throwVelocity = 7f;

    private CircularMovement m;

    private void Start()
    {
        m = GetComponent<CircularMovement>();
    }

    public void OnSnowballThrow(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            var snowballing = Instantiate(snowball, throwPoint.position, throwPoint.rotation);
            snowballing.GetComponent<Rigidbody>().velocity = m.FromLocal(new Vector2(throwVelocity, throwVelocity));
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        snowball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }*/
}
