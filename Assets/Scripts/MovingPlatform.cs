using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Muls lossts bitte als Transforms
    // Es isch anfoch viel gschickta as wie la pura Vector3s
    [SerializeField] private Transform[] positions;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxAcceleration;

    [SerializeField] private float uprightSpringStrength;
    [SerializeField] private float uprightSpringDamper;

    [SerializeField] private GameObject indicator;
    [SerializeField] private float indicatorSpacing;
    
    private int _currentIndex;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 current = positions[i].position;
            Vector3 next = positions[(i + 1) % positions.Length].position;
            float dis = Vector3.Distance(current, next);
            int amount = (int)(dis / indicatorSpacing);
            float magnitude = dis / amount;
            for (int j = 0; j < amount; j++)
            {
                Instantiate(indicator, current + (next - current).normalized * j * magnitude, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateSpringForce();
        UpdateUprightForce();
    }
    
    private void UpdateSpringForce()
    {
        Vector3 pos = transform.position;
        
        Vector3 targetPosition = positions[_currentIndex].position;

        Vector3 move = (targetPosition - pos).normalized;

        Vector3 goalVel = move * maxSpeed;
        goalVel = Vector3.MoveTowards(_rigidbody.velocity, goalVel, acceleration * Time.fixedDeltaTime);

        Vector3 neededAccel = (goalVel - _rigidbody.velocity) / Time.fixedDeltaTime;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAcceleration);

        _rigidbody.AddForce(neededAccel);
        
        Debug.DrawLine(pos, pos + neededAccel, Color.green);

        if (Vector3.Distance(pos, targetPosition) < 0.2f)
        {
            _currentIndex = (_currentIndex + 1) % positions.Length;
        }
    }

    private void UpdateUprightForce()
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = PlayerMovement.ShortestRotation(Quaternion.identity, characterCurrent);

        toGoal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        _rigidbody.AddTorque(rotAxis * (rotRadians * uprightSpringStrength) -
                             _rigidbody.angularVelocity * uprightSpringDamper);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawLine(positions[i].position, positions[(i + 1) % positions.Length].position);
        }
    }
}