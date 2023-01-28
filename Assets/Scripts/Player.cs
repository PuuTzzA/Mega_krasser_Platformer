using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CircularMovement _m;
    private Rigidbody _rb;
    private bool _grounded;
    private int _direction;
    private int _movement;
    private Vector2 _moveVector;
    private bool _touchingWall;
    private bool _hasSecondJump;

    private bool _dashing;
    private bool _hasAirDash = true;
    private float _groundDashCooldown;

    private float _baseAirAcc = 80.0f;
    private float _airAcc;

    public float speed = 8.0f;
    public GameObject groundChecker, wallChecker;
    private int coinsAdd;
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>().x;
        if (value > 0.4f)
            _movement = 1;
        else if (value < -0.4f)
            _movement = -1;
        else
            _movement = 0;
        if (_movement != 0)
        {
            _direction = _movement;
            _m.SetFrontFacing(_direction == 1 ? true : false);
            _movement = Mathf.Abs(_movement);
            wallChecker.GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            wallChecker.GetComponent<CapsuleCollider>().enabled = false;
            _touchingWall = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_grounded)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = 15.0f;
                _rb.velocity = _m.FromLocal(velocity);
                GroundCheckerEnabled(false);
                _grounded = false;
                StartCoroutine(JumpCoroutine());
            }
            else if (_touchingWall)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = 13.0f;
                velocity.x = -13.0f;
                _rb.velocity = _m.FromLocal(velocity);
                _airAcc = 0.0f;
                StartCoroutine(WallJumpCoroutine());
            }
            else if (_hasSecondJump)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = 15.0f;
                _rb.velocity = _m.FromLocal(velocity);
                GroundCheckerEnabled(false);
                _grounded = false;
                _hasSecondJump = false;
                StartCoroutine(JumpCoroutine());
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && (_grounded ? _groundDashCooldown <= 0 : _hasAirDash))
        {
            var velocity = _m.ToLocal(_rb.velocity);
            velocity.x = speed * 4;
            _rb.velocity = _m.FromLocal(velocity);
            StartDashing();
            StartCoroutine(DashCoroutine());
            if (_grounded)
                _groundDashCooldown = 2;
            else
                _hasAirDash = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _m = GetComponent<CircularMovement>();
        _rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.right * 15;
        _direction = 1;
        _hasSecondJump = true;

        _airAcc = _baseAirAcc;
    }


    void Update()
    {
        if (!_dashing)
        {
            var velocity = _m.ToLocal(_rb.velocity);
            if (!_grounded)
            {

                float targetSpeed = speed * _movement;
                if (Mathf.Abs(targetSpeed - velocity.x) <= _airAcc * Time.deltaTime)
                {
                    velocity.x = targetSpeed;
                }
                else
                {
                    float factor = 1;
                    if (targetSpeed < velocity.x)
                        factor = -1;
                    velocity.x += _airAcc * Time.deltaTime * factor;
                }

            }
            /**/
            else
            {
                velocity.x = speed * _movement * _moveVector.x;
                velocity.y = speed * _movement * _moveVector.y;
            }

            if (_touchingWall)
            {
                velocity.y = Mathf.Max(velocity.y, -3.0f);
            }/**/
            velocity.x = Mathf.Clamp(velocity.x, -speed, speed);
            velocity.y = Mathf.Clamp(velocity.y, -30.0f, 30.0f);
            _rb.velocity = _m.FromLocal(velocity);

            //_rb.AddForce(_m.fromLocal(2.0f * _movement * _moveVector), ForceMode.VelocityChange);
        }
        else
        {
            var velocity = _m.ToLocal(_rb.velocity);
            velocity.x -= 1.0f;
            _rb.velocity = _m.FromLocal(velocity);
        }
        if (_groundDashCooldown >= 0)
            _groundDashCooldown -= Time.deltaTime;
    }

    public void SetGrounded(bool grounded)
    {
        this._grounded = grounded;
        if (_grounded == true)
        {
            this._hasSecondJump = true;
            this._hasAirDash = true;
        }
    }

    public void GroundCheckerEnabled(bool enabled)
    {
        groundChecker.GetComponent<CapsuleCollider>().enabled = enabled;
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    public void SetTouchingWall(bool touchingWall)
    {
        this._touchingWall = touchingWall;
    }

    public bool IsTouchingWall()
    {
        return _touchingWall;
    }

    public void SetMoveVector(Vector2 vec)
    {
        _moveVector = vec;
    }

    public IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        GroundCheckerEnabled(true);
    }

    public IEnumerator WallJumpCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        _airAcc = 0.2f * _baseAirAcc;
        yield return new WaitForSeconds(0.25f);
        _airAcc = 0.5f * _baseAirAcc;
        yield return new WaitForSeconds(0.1f);
        _airAcc = 1.0f * _baseAirAcc;
    }

    public IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        EndDashing();
    }

    public void StartDashing()
    {
        _dashing = true;
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.useGravity = false;
        GroundCheckerEnabled(false);
    }

    public void EndDashing()
    {
        _dashing = false;
        _rb.useGravity = true;
        GroundCheckerEnabled(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_dashing)
            EndDashing();
    }

    public void addCoin()
    {
        coinsAdd++; 

    }

    public void damage()
    {
        Debug.Log("DAMAGE!!");
    }
}
