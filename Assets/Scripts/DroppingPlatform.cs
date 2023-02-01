using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    private Vector3 StartPosition;

    private bool _isFalling;

    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float fallDuration = 2.0f;
    [SerializeField] private float respawnDelay = 2.0f;

    private void Start()
    {
        StartPosition = transform.localPosition;
        //GetComponent<Rigidbody>().AddForce(new Vector3(0, -2, 0), ForceMode.Force);
    }

    private void Update() {
        if(_isFalling)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -0.05f, 0), ForceMode.VelocityChange);
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
        _isFalling = true;
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(fallDuration);
        _isFalling = false;
        SetComponentsEnabled(false);

        Debug.Log("Startpos: " + StartPosition);
        yield return new WaitForSeconds(respawnDelay);
        transform.position = StartPosition;
        SetComponentsEnabled(true);
        GetComponent<Rigidbody>().isKinematic = true;

    }
    

    private void SetComponentsEnabled(bool enabled)
    {
        GetComponent<MeshRenderer>().enabled = enabled;
        GetComponent<BoxCollider>().enabled = enabled;
        
    }
}
