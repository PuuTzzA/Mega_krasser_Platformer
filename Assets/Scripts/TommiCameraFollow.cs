using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TommiCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followedObject;
    [SerializeField] private float distanceAway, startVertical, offsetUp;
    [SerializeField] private float smooth;

    [SerializeField] private float verticalSpeed;
    [SerializeField] private float verticalMin;
    [SerializeField] private float verticalMax;

    private float _verticalMouse;
    private float _vertical;
    private PlayerMovement _pm;

    private void Awake()
    {
        _vertical = startVertical;
        _pm = followedObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _verticalMouse = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        float y = _pm.toGoalRotation.eulerAngles.y * Mathf.Deg2Rad;

        _vertical += _verticalMouse * verticalSpeed;
        _vertical = _vertical >= verticalMax ? verticalMax : _vertical;
        _vertical = _vertical <= verticalMin ? verticalMin : _vertical;

        // Only consider y rotation, because otherwise there is strong screen shake if you bump into Objects and the 
        // player is not rotated perfectly upwards
        Vector3 toPosition = followedObject.position -
                             new Vector3(MathF.Sin(y), 0, Mathf.Cos(y)) * (Mathf.Sin(_vertical * Mathf.Deg2Rad) * distanceAway) +
                             Vector3.up * (Mathf.Cos(_vertical * Mathf.Deg2Rad) * distanceAway);
        transform.position = Vector3.Lerp(transform.position, toPosition, smooth * Time.deltaTime);
        transform.LookAt(followedObject.position + new Vector3(0, offsetUp, 0));
    }
}