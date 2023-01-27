using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    private float rotation = 1f;
    [SerializeField] private GameObject coin;
    void Update()
    {
       transform.Rotate(0, rotation, 0,Space.World);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        { Destroy(coin);
        }
    }
}
