using UnityEngine;

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

    [Header("Slope Handling")] [SerializeField]
    private float maxSlopeAngle;


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

    // Sliding based on:
    // https://www.youtube.com/watch?v=SsckrYYxcuM&ab_channel=Dave%2FGameDevelopment
    [Header("Sliding")] 
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftShift;

    [Header("Raycast to check if grounded")] [SerializeField]
    private float rayLength;

    [Header("References for other Scripts")]
    public Quaternion toGoalRotation;


    private Rigidbody _rigidbody;

    private Vector3 _otherVel;

    private float _horizontalInput;
    private float _verticalInput;
    private float _turnInput;
    private float _jumpInput;

    private bool _onSlope;
    private Vector3 _slopeNormal;

    private bool _jumpInputPrev;
    private float _jumpTime;
    private bool _jumping;

    private float _timeSinceLeavingGround;
    private bool _jumpPressedInAir;

    private bool _activeGrapple;
    private bool _sliding;

    private float _slideTimer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        toGoalRotation = Quaternion.identity;
    }

    private void Update()
    {
        GetInput();
        if (Input.GetKeyDown(slideKey))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && _sliding)
        {
            StopSlide();
        }
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

        if (_onSlope)
        {
            // to stick properly on the slope
            _rigidbody.AddForce(Vector3.down * 30, ForceMode.Acceleration);
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

        // Sliding
        if (_sliding)
        {
            SlidingMovement();
        }
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

            // check if on slope
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            _onSlope = 0.1f < slopeAngle && slopeAngle < maxSlopeAngle;
            _slopeNormal = hit.normal;
        }
        else
        {
            // not on slope
            _onSlope = false;

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
            if (_sliding)
            {
                float amtToRotate = _turnInput * Time.deltaTime * turnSpeed;
                toGoalRotation = Quaternion.Euler(toGoalRotation.eulerAngles.x, toGoalRotation.eulerAngles.y,
                    toGoalRotation.eulerAngles.z - amtToRotate);
            }
            else
            {
                float amtToRotate = _turnInput * Time.deltaTime * turnSpeed;
                toGoalRotation *= Quaternion.AngleAxis(amtToRotate, Vector3.up);
            }
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
        Vector3 move = _sliding
            ? transform.TransformDirection(new Vector3(_horizontalInput, _verticalInput, 0))
            : transform.TransformDirection(new Vector3(_horizontalInput, 0, _verticalInput));

        // If on slope
        if (_onSlope && _otherVel.magnitude < 0.01f)
        {
            move = GetSlopeMovementDirection(move);
        }

        // Limit the speed to a magnitude of one
        // Prevents that diagonal walking is faster than straight walking
        move = move.magnitude > 1 ? move.normalized : move;

        // If the player is or was on a moving Platform
        move += _otherVel / maxRunSpeed;

        float velDot = Vector3.Dot(_rigidbody.velocity.normalized, move.normalized);
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

    private void StartSlide()
    {
        _sliding = true;
        toGoalRotation *= Quaternion.Euler(90, 0, 0);
        _slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        if (!_onSlope)
        {
            // transform.up because you are lying when sliding
            _rigidbody.AddForce(transform.up * slideForce);

            _slideTimer -= Time.fixedDeltaTime;
            
            if (_slideTimer <= 0)
            {
                StopSlide();
            }
            return;
        }
        if (_rigidbody.velocity.y > 0)
        {
            _rigidbody.AddForce(GetSlopeMovementDirection(transform.up)* slideForce);

            _slideTimer -= Time.fixedDeltaTime;
            
            if (_slideTimer <= 0)
            {
                StopSlide();
            }
            return;
        }
        // On slopes, going down you get infinite sliding time
        _rigidbody.AddForce(Vector3.down * 200, ForceMode.Acceleration);
        _rigidbody.AddForce(GetSlopeMovementDirection(transform.up) * slideForce);
    }

    private void StopSlide()
    {
        _sliding = false;
        toGoalRotation *= Quaternion.Euler(-90, 0, 0);
    }

    private Vector3 GetSlopeMovementDirection(Vector3 movementDirection)
    {
        return Vector3.ProjectOnPlane(movementDirection, _slopeNormal).normalized;
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