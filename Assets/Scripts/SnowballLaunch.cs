using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SnowballLaunch : MonoBehaviour
{
  public Transform throwPoint;
  public GameObject snowball;
  private float throwVelocity= 7f;

  private CircularMovement m;

  private void Start()
  {
      m = GetComponent<CircularMovement>();
  }

  private void Update()
  { if (Input.GetButtonDown("Fire1")){
          var snowballing = Instantiate(snowball, throwPoint.position, throwPoint.rotation);
          snowballing.GetComponent<Rigidbody>().velocity = m.FromLocal(new Vector2(throwVelocity, throwVelocity));
          
          
      }
      


  }

  /*private void OnTriggerEnter(Collider other)
  {
      snowball.GetComponent<Rigidbody>().velocity = Vector3.zero;
  }*/
}
