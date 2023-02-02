using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGoatKnockback : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && transform.parent.gameObject.GetComponent<MountainGoat>()._state != MountainGoat.State.STUNNED)
        {
            player.GetComponent<Player>().damage();
        }
    }
    
}
