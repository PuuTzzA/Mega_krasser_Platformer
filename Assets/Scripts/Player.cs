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

    private float _groundAcc;
    private float _airAcc;

    public float speed = 10.0f;
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
                groundChecker.GetComponent<SphereCollider>().enabled = false;
                _grounded = false;
                StartCoroutine(JumpCoRoutine());
            }
            else if (_touchingWall)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = 15.0f;
                velocity.x = -15.0f;
                _rb.velocity = _m.FromLocal(velocity);
            }
            else if (_hasSecondJump)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = 15.0f;
                _rb.velocity = _m.FromLocal(velocity);
                groundChecker.GetComponent<SphereCollider>().enabled = false;
                _grounded = false;
                _hasSecondJump = false;
                StartCoroutine(JumpCoRoutine());
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var velocity = _m.ToLocal(_rb.velocity);
            velocity.x = speed * 4;
            _rb.velocity = _m.FromLocal(velocity);
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
    }


    void FixedUpdate()
    {
        Debug.Log(_grounded);
        var velocityPrevious = _m.ToLocal(_rb.velocity);

        float targetSpeed = speed * _movement;

        //velocityPrevious.x 

        _rb.AddForce(_m.FromLocal(new Vector2(_grounded ? _groundAcc : _airAcc, 0.0f) * _movement), ForceMode.VelocityChange);

        var velocity = _m.ToLocal(_rb.velocity);
        /**/
        if (_grounded)
        {
            velocity.x = speed * _movement * _moveVector.x;
            velocity.y = speed * _movement * _moveVector.y;
        }

        if (_touchingWall)
        {
            velocity.y = Mathf.Max(velocity.y, -3.0f);
        }/**/
        velocity.x = Mathf.Clamp(velocity.x, -10.0f, 10.0f);
        velocity.y = Mathf.Clamp(velocity.y, -30.0f, 30.0f);
        _rb.velocity = _m.FromLocal(velocity);

        //_rb.AddForce(_m.fromLocal(2.0f * _movement * _moveVector), ForceMode.VelocityChange);

    }

    public void SetGrounded(bool grounded)
    {
        this._grounded = grounded;
        if (_grounded == true)
            this._hasSecondJump = true;
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

    public IEnumerator JumpCoRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        groundChecker.GetComponent<SphereCollider>().enabled = true;
    }

    public void addCoin()
    {
        coinsAdd++; 

    }

    public void damage()
    {

    }
}
