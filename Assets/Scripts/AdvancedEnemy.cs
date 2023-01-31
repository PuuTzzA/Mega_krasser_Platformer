using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEnemy : MonoBehaviour
{
    private enum Behaviour
    {
        Chase,
        Intercept,
        Patrol,
        ChasePatrol,
        Hide
    }

    [SerializeField] private float gravity;

    [Header("Hovering above the ground")] [SerializeField]
    private float hoverHeight;

    [SerializeField] private float hoverSpringStrength;
    [SerializeField] private float hoverSpringDamper;

    [Header("Stay in a upright rotation")] [SerializeField]
    private float uprightSpringStrength;

    [SerializeField] private float uprightSpringDamper;

    [Header("Movement")] [SerializeField] private float maxRunSpeed;
    [SerializeField] private float maxChaseSpeed;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float maxRunAcceleration;
    [SerializeField] private AnimationCurve accelerationFactor;

    [Header("Raycast to check if grounded")] [SerializeField]
    private float rayLength;

    [Header("Reference to player")] [SerializeField]
    private Rigidbody prey;

    private Behaviour _behaviour;
    private Rigidbody _rigidbody;

    private Vector3 _moveDirection;
    private float _currentMaxSpeed;
    private Vector3 _otherVel;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _behaviour = Behaviour.Chase;
    }

    private void FixedUpdate()
    {
        switch (_behaviour)
        {
            case Behaviour.Chase:
                Chase(prey.position);
                break;
            case Behaviour.Intercept:
                Intercept(prey.position, prey.velocity);
                break;
            case Behaviour.Patrol:
                break;
            case Behaviour.ChasePatrol:
                break;
            case Behaviour.Hide:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Hover above the ground at fixed Height
        UpdateHoverForce();
        // Say upright
        UpdateUprightForce();
        // Move according to _moveDirection and _currentMaxSpeed
        Move();
    }

    private void Chase(Vector3 targetPosition)
    {
        _moveDirection = targetPosition - _rigidbody.position;
        _moveDirection.y = 0;
        _moveDirection = _moveDirection.normalized;
        _currentMaxSpeed = maxChaseSpeed;
    }

    private void Intercept(Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 velocityRelative = targetVelocity - prey.velocity;
        velocityRelative = velocityRelative.magnitude < 0.001 ? new Vector3(1, 0, 1) : velocityRelative;
        float distance = Vector3.Distance(targetPosition, _rigidbody.position);
        float timeToClose = distance / velocityRelative.magnitude;
        Vector3 predictedInterceptionPoint = targetPosition + timeToClose * targetVelocity;
        Chase(predictedInterceptionPoint);
    }

    private void UpdateHoverForce()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
            Vector3 vel = _rigidbody.velocity;
            Vector3 rayDir = Vector3.down;

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = hit.distance - hoverHeight;

            float springForce = x * hoverSpringStrength - relVel * hoverSpringDamper;

            // Debug.DrawLine(transform.position, transform.position + rayDir * springForce, Color.yellow);


            _rigidbody.AddForce(rayDir * springForce);


            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
                // Debug.DrawLine(transform.position, transform.position + otherVel);
                _otherVel = otherVel;
            }
            else
            {
                _otherVel = Vector3.zero;
            }
        }
        else
        {
            // Additional Gravity
            _rigidbody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
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

    private void Move()
    {
        // Limit the speed to a magnitude of one
        // Prevents that diagonal walking is faster than straight walking
        _moveDirection = _moveDirection.magnitude > 1 ? _moveDirection.normalized : _moveDirection;

        // If the player is or was on a moving Platform
        _moveDirection += _otherVel / maxRunSpeed;

        float velDot = Vector3.Dot(_rigidbody.velocity.normalized, _moveDirection.normalized);
        float accel = runAcceleration * accelerationFactor.Evaluate(velDot);

        Vector3 goalVel = _moveDirection * _currentMaxSpeed;
        goalVel = Vector3.MoveTowards(_rigidbody.velocity, goalVel, accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (goalVel - _rigidbody.velocity) / Time.fixedDeltaTime;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxRunAcceleration);

        _rigidbody.AddForce(neededAccel);
    }
}