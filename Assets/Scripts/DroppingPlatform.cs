using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    private Vector3 StartPosition;

    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float fallDuration = 2.0f;
    [SerializeField] private float respawnDelay = 2.0f;

    private void Start()
    {
       StartPosition = transform.position;
        GetComponent<Rigidbody>().AddForce(new Vector3(0, -2, 0), ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(drop()); 
        }
    }

    private IEnumerator drop()
    {
        yield return new WaitForSeconds(fallDelay);
        Debug.Log("drop");
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(fallDuration);
        SetComponentsEnabled(false);
        transform.position = StartPosition;
        yield return new WaitForSeconds(respawnDelay);
        SetComponentsEnabled(true);
        GetComponent<Rigidbody>().isKinematic = true;

    }
    

    private void SetComponentsEnabled(bool enabled)
    {
        GetComponent<MeshRenderer>().enabled = enabled;
        GetComponent<BoxCollider>().enabled = enabled;
        
    }
}
