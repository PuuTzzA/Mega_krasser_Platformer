using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    // Based on:
    // https://www.youtube.com/watch?v=TYzZsBl3OI0&ab_channel=Dave%2FGameDevelopment

    [Header("References")] [SerializeField]
    private Transform cam;

    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask layerMaskGrapple;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Grappling")] [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float grapplingCooldown;
    [SerializeField] private float overshootYAxis;
    
    [Header("Input")] 
    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse0;

    private Vector3 _grapplePoint;
    private float _cooldownTimer;
    private bool _grappling;

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        // Debug.DrawLine(cam.position, cam.position + cam.forward * maxGrappleDistance, Color.black);
    }

    private void LateUpdate()
    {
        if (_grappling)
        {
            lineRenderer.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (_cooldownTimer > 0)
        {
            return;
        }

        _grappling = true;

        // Start the ray at the position of the position of the player away from the camera
        if (Physics.Raycast(cam.position + cam.forward * Vector3.Distance(cam.position, transform.position), cam.forward, out RaycastHit hit, maxGrappleDistance, layerMaskGrapple))
        {
            _grapplePoint = hit.point;
            
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            _grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, _grapplePoint);
    }

    private void ExecuteGrapple()
    {
        Vector3 pos = transform.position;
        Vector3 lowestPoint = new Vector3(pos.x, pos.y - 1f, pos.z);
        float grapplePointRelativeYPos = _grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }
        
        GetComponent<PlayerMovement>().JumpToPosition(_grapplePoint, highestPointOnArc);
        
        // If you dont collide with nothing, stop the grapple after 2 seconds
        Invoke(nameof(StopGrapple), 2f);
    }

    public void StopGrapple()
    {
        _grappling = false;

        _cooldownTimer = grapplingCooldown;

        lineRenderer.enabled = false;
    }
}