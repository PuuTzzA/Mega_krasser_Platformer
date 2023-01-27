using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGoatKnockback : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<Player>().damage();
        }
    }
    
}
