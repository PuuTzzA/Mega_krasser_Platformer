using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGoatKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackStrength;

    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = transform.parent.GetComponent<MountainGoat>().player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            Debug.Log(_player.GetComponent<Rigidbody>());
            Vector3 force = (transform.position - _player.transform.position).normalized;
            force = new Vector3(force.x, 0, force.y);
            _player.GetComponent<Rigidbody>().AddForce(force * knockbackStrength, ForceMode.Impulse);
        }
    }
}
