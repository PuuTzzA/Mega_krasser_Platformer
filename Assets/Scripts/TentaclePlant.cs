using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentaclePlant : MonoBehaviour
{
    [SerializeField] private GameObject tentacle;
    [SerializeField] private GameObject player;

    [Header("Tentacle Settings, get passed on to individual Tentacles")]
    [SerializeField] private int tentacleAmount;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float grappleStrength;
    [SerializeField] private float breakFreeDistance;
    [SerializeField] private float maxLength;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tentacleAmount; i++)
        {
            GameObject temp = Instantiate(tentacle, transform.position, Quaternion.identity);
            temp.gameObject.transform.parent = transform;
            TentaclePlantTentacle tempScript = temp.GetComponent<TentaclePlantTentacle>();
            tempScript.player = player;
            tempScript.detectionRadius = detectionRadius;
            tempScript.grappleStrength = grappleStrength;
            tempScript.breakFreeDistance = breakFreeDistance;
            tempScript.maxLength = maxLength;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("remove Live");
            player.GetComponent<Player>().damage();
        }
    }
}