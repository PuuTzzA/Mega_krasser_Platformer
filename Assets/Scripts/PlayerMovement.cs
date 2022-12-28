using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    // Based on:
    // https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames

    [SerializeField] private float gravity;

    [Header("Hovering above the ground")] [SerializeField]
    private float hoverHeight;

    [SerializeField] private float hoverSpringStrength;
    [SerializeField] private float hoverSpringDamper;

    [Header("Stay in a upright rotation")] [SerializeField]
    private float uprightSpringStrength;

    [SerializeField] private float uprightSpringDamper;

    [Header("Movement")] [SerializeField] private float turnSpeed;
    [SerializeField] private float maxRunSpeed;
    [SerializeField] private float runAcceleration;
    [SerializeField] private float maxRunAcceleration;
    [SerializeField] private AnimationCurve accelerationFactor;

    [Header("Jumping")]
    // Toggle between "normal" and variable jump (variable jump >>>>>> normal jump)
    [SerializeField]
    private bool normalJump;

    // Variable jump
    [SerializeField] private float jumpMaxAcceleration;
    [SerializeField] private float jumpMinAcceleration;

    [SerializeField] private float jumpMaxDuration;

    // Normal jump (always the same height)
    [SerializeField] private float jumpImpulseStrength;

    [SerializeField] private float coyoteTime; // <- allows player to jump, even if he is not grounded

    [Header("Raycast to check if grounded")] 
    [SerializeField] private float rayLength;

    [Header("References for other Scripts")]
    public Quaternion toGoalRotation;

    
    private Rigidbody _rigidbody;
    
    private Vector3 _otherVel;

    private float _horizontalInput;
    private float _verticalInput;
    private float _turnInput;
    private float _jumpInput;

    private bool _jumpInputPrev;
    private float _jumpTime;
    private bool _jumping;

    private float _timeSinceLeavingGround;
    private bool _jumpPressedInAir;

    private bool _activeGrapple;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        toGoalRotation = Quaternion.identity;
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Mouse X");
        _jumpInput = Input.GetAxisRaw("Jump");
    }

    private void FixedUpdate()
    {
        if (_activeGrapple)
        {
            _rigidbody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
            return;
        }

        // Hover above the ground at fixed Height
        UpdateHoverForce();
        // If there is a turn input, set the desired rotation according to the input
        Turn();
        // Turn to the desired rotation
        UpdateUprightForce();
        // If there is a movement input add Forces in the desired direction
        Move();
        // If there is a jump input, add upwards Force as long as the inputKey is pressed
        // Toggle between "normal" and variable jump (variable jump >>>>>> normal jump)
        if (normalJump)
        {
            // Add an impulse, always the same
            JumpNormal();
        }
        else
        {
            // Add force as long as jump key is pressed or the max Jump time is over
            JumpVariable();
        }
    }

    private void UpdateHoverForce()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
            Vector3 vel = _rigidbody.velocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down);

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

            if (!_jumping)
            {
                _rigidbody.AddForce(rayDir * springForce);
            }

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

            _timeSinceLeavingGround = 0;
        }
        else
        {
            // Additional Gravity
            _rigidbody.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);

            // Keep Momentum when jumping from a moving platform
            _otherVel = new Vector3(_otherVel.x, 0, _otherVel.z);

            // For coyote Time
            _timeSinceLeavingGround += Time.fixedDeltaTime;

            // Input buffering
            if (_jumpInput > 0)
            {
                _jumpPressedInAir = true;
            }

            if (_jumpInput < 1 && _jumpInputPrev)
            {
                _jumpPressedInAir = false;
            }
        }
    }

    private void Turn()
    {
        if (Mathf.Abs(_turnInput) > 0)
        {
            float amtToRotate = _turnInput * Time.deltaTime * turnSpeed;
            toGoalRotation *= Quaternion.AngleAxis(amtToRotate, Vector3.up);
        }
    }

    private void UpdateUprightForce()
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = ShortestRotation(toGoalRotation, characterCurrent);

        toGoal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        _rigidbody.AddTorque(rotAxis * (rotRadians * uprightSpringStrength) -
                             _rigidbody.angularVelocity * uprightSpringDamper);
    }

    private void Move()
    {
        Vector3 move = transform.TransformDirection(new Vector3(_horizontalInput, 0, _verticalInput)) +
                       _otherVel / maxRunSpeed; // <- If the player is or was on a moving Platform

        float velDot = Vector3.Dot(_rigidbody.velocity.normalized, move);
        float accel = runAcceleration * accelerationFactor.Evaluate(velDot);

        Vector3 goalVel = move * maxRunSpeed;
        goalVel = Vector3.MoveTowards(_rigidbody.velocity, goalVel, accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (goalVel - _rigidbody.velocity) / Time.fixedDeltaTime;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxRunAcceleration);

        _rigidbody.AddForce(neededAccel);
    }

    private void JumpNormal()
    {
        // jumping with coyote Time and input buffering to allow for imprecise inputs 
        if (((_jumpInput > 0 && !_jumpInputPrev) || _jumpPressedInAir) && _timeSinceLeavingGround < coyoteTime &&
            !_jumping)
        {
            _jumpTime = 0;
            _jumping = true;
            _jumpPressedInAir = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(new Vector3(0, jumpImpulseStrength, 0), ForceMode.Impulse);
        }

        if (_jumping && _rigidbody.velocity.y < 0)
        {
            _jumping = false;
        }

        _jumpInputPrev = _jumpInput > 0;
    }

    private void JumpVariable()
    {
        // jumping with coyote Time and input buffering to allow for imprecise inputs 
        if (((_jumpInput > 0 && !_jumpInputPrev) || _jumpPressedInAir) && _timeSinceLeavingGround < coyoteTime &&
            !_jumping)
        {
            _jumpTime = 0;
            _jumping = true;
            _jumpPressedInAir = false;
        }

        if (_jumping)
        {
            _jumpTime += Time.fixedDeltaTime;

            float acc = MapRange(_jumpTime, jumpMaxDuration, 0, jumpMinAcceleration, jumpMaxAcceleration);
            _rigidbody.AddForce(acc * Vector3.up, ForceMode.Acceleration);

            if (_jumpTime > jumpMaxDuration || (_jumpInput < 1 && _jumpInputPrev))
            {
                _jumpTime = 0;
                _jumping = false;
            }
        }

        _jumpInputPrev = _jumpInput > 0;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        _activeGrapple = true;
        _rigidbody.velocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_activeGrapple)
        {
            _activeGrapple = false;
            GetComponent<Grappling>().StopGrapple();
        }
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * trajectoryHeight * -gravity);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / -gravity)
                                               + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / -gravity));

        return velocityY + velocityXZ;
    }

    private static float MapRange(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (outMax - outMin) / (inMax - inMin) * (value - inMin);
    }

    // Shortest Rotation Calculation from: 
    // https://forum.unity.com/threads/shortest-rotation-between-two-quaternions.812346/
    public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
        {
            return a * Quaternion.Inverse(Multiply(b, -1));
        }

        return a * Quaternion.Inverse(b);
    }

    private static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
}