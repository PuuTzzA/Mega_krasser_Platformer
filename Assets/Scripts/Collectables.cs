using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coin touched other " + other.ToString());
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().addCoin();
            Destroy(gameObject);
        }
    }
}
