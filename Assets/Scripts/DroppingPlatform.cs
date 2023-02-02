using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    private Vector3 StartPosition;

    private bool _isFalling;

    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float fallDuration = 2.0f;
    [SerializeField] private float respawnDelay = 2.0f;
    [SerializeField] private Material hiddenMaterial;

    private GameObject _indicator;
    private GameObject _indicatorTemp;
    private bool _isActivated;

    private void Start()
    {
        StartPosition = transform.localPosition;
        _indicator = new GameObject();
        _indicator.AddComponent<MeshRenderer>().material = hiddenMaterial;
        _indicator.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        //GetComponent<Rigidbody>().AddForce(new Vector3(0, -2, 0), ForceMode.Force);
    }

    private void Update() {
        if(_isFalling)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -0.05f, 0), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !_isActivated)
        {
            _isActivated = true;
            StartCoroutine(drop()); 
        }
    }

    private IEnumerator drop()
    {
        yield return new WaitForSeconds(fallDelay);
        Debug.Log("drop");
        _isFalling = true;
        Destroy(_indicatorTemp);
        _indicatorTemp = Instantiate(_indicator, transform.position, transform.rotation);
        _indicatorTemp.transform.localScale = transform.localScale;
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(fallDuration);
        _isFalling = false;
        SetComponentsEnabled(false);

        Debug.Log("Startpos: " + StartPosition);
        yield return new WaitForSeconds(respawnDelay);
        transform.position = StartPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        SetComponentsEnabled(true);
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(_indicatorTemp);
        _isActivated = false;
    }
    

    private void SetComponentsEnabled(bool enabled)
    {
        GetComponent<MeshRenderer>().enabled = enabled;
        GetComponent<BoxCollider>().enabled = enabled;
        
    }
}
