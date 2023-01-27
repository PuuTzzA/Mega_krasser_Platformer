using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject endUI;

    private bool _triggered;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !_triggered)
        {
            player.GetComponent<Player>().enabled = false;
            player.GetComponent<Rigidbody>().isKinematic = true;
            Instantiate(endUI);
            _triggered = true;
        }
    }
}
