using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody _rigidbody;
    public GameObject despawnZone;
    public GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.TransformDirection(Vector3.up * speed);
        transform.rotation *= UnityEngine.Quaternion.Euler(-90, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == despawnZone)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            // Remove Lives
            Debug.Log("Remove Lives");
            Destroy(gameObject);
        }
    }
}
