using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum State
    {
        IDLE,
        RUNNING,
        AIRBORNE,
        DASHING,
        WALLSLIDING,

    };


    private CircularMovement _m;
    private Rigidbody _rb;
    private State _state;
    private bool _grounded;
    private int _direction;
    private int _movement;

    public float speed = 10.0f;

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
            _direction = _movement;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && _grounded)
        {
            var velocity = _m.toLocal(_rb.velocity);
            velocity.y = 15;
            _rb.velocity = _m.fromLocal(velocity);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var velocity = _m.toLocal(_rb.velocity);
            velocity.x = speed * _direction * 4;
            _rb.velocity = _m.fromLocal(velocity);
            SetState(State.DASHING);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _m = GetComponent<CircularMovement>();
        _rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.right * 15;

        _state = State.IDLE;
        _direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //if (_state != State.DASHING)
        //{
            var velocity = _m.toLocal(_rb.velocity);
            velocity.x = speed * _movement;
            _rb.velocity = _m.fromLocal(velocity);
        //}
    }

    public void OnStateEnd(State state)
    {

    }

    public void OnStateBegin(State state)
    {
        switch(state)
        {

        }
    }

    public void SetState(State newState)
    {
        OnStateEnd(_state);
        _state = newState;
        OnStateEnd(_state);
    }

    public State GetState()
    {
        return _state;
    }

    public void SetGrounded(bool grounded)
    {
        this._grounded = grounded;
    }

    public bool GetGrounded()
    {
        return _grounded;
    }
}
