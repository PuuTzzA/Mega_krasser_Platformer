using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Collectables : MonoBehaviour
{
    public AudioClip coinSound; 

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coin touched other " + other.ToString());
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().addCoin();
            Debug.Log("AUDIO SHOULD PLay");
            
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(gameObject);
        }
    }
}
