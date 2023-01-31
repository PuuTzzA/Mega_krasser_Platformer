using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    private Vector3 StartPosition;
    private void Start()
    {
       StartPosition = platform.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(drop()); 
        }
    }

    private IEnumerator drop()
    {
        yield return new WaitForSeconds(1f);
        platform.GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(3f); SetComponentsEnabled(false);
        yield return new WaitForSeconds(1f);
        platform.transform.position = StartPosition;
        SetComponentsEnabled(true);   platform.GetComponent<Rigidbody>().isKinematic = true;

    }
    

    private void SetComponentsEnabled(bool enabled)
    {
        platform.GetComponent<MeshRenderer>().enabled = enabled;
        platform.GetComponent<BoxCollider>().enabled = enabled;
        
    }
}
