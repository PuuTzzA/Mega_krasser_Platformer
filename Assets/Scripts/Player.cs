using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private CircularMovement _m;
    private Rigidbody _rb;
    private UIIngame _ingameUI;

    private bool _grounded;
    private int _direction;
    private int _movement;
    private bool _touchingWall;
    private float _remainingCoyoteTime;

    private IEnumerator _currentWalljump;

    private bool _hasSecondJump;
    private float _airAcc;

    private bool _dashing;
    private bool _hasAirDash = true;
    private float _groundDashRemainingCooldown;
    private float _dashSpeed = 40.0f;
    private float _currentDashSpeed;
    private bool _freezeDirection;

    private bool _invisFrames;

    private int _coins;

    private int _health;


    private float _speed = 8.0f;
    private float _jumpSpeed = 15.0f;
    private Vector2 _wallJumpSpeed = new Vector2(-13.0f, 13.0f);
    private float _wallSlideSpeed = 3.0f;
    private float _baseAirAcc = 80.0f;
    private float _groundDashCooldown = 2.0f;
    private int _maxHealth = 3;
    private float _invisFramesDuration = 2.0f;
    private float _deathDepth = 20;
    private float _coyoteTime = 0.1f;

    public GameObject wallChecker, uiIngameObj, endScreenPrefab, winScreenPrefab, pausePrefab;
    private bool _isDead;

    private bool _isPaused;
    private float _time;

    [SerializeField]
    private int levelnumber;

    private bool _triggered;


    public void OnRetry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started && !_isDead && !_isPaused)
        {
            _isPaused = true;
            GetComponent<CenterMouse>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            GameObject o = Instantiate(pausePrefab);
            o.GetComponent<UIPause>().player = this;
        }
    }

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
            if (!_freezeDirection)
            {
                _direction = _movement;
                _m.SetDirection(_direction);
            }
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
            if (_grounded || _remainingCoyoteTime > 0)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = _jumpSpeed;
                _rb.velocity = _m.FromLocal(velocity);
                SetGrounded(false);
                _remainingCoyoteTime = -1;
            }
            else if (_touchingWall)
            {
                _freezeDirection = true;
                _direction = -_direction;
                _m.SetDirection(_direction);
                var velocity = _wallJumpSpeed;
                _rb.velocity = _m.FromLocal(velocity);
                _airAcc = 0.0f;
                if (_currentWalljump != null)
                    StopCoroutine(_currentWalljump);
                _currentWalljump = WallJumpCoroutine();
                StartCoroutine(_currentWalljump);
            }
            else if (_hasSecondJump)
            {
                var velocity = _m.ToLocal(_rb.velocity);
                velocity.y = _jumpSpeed;
                _rb.velocity = _m.FromLocal(velocity);
                SetGrounded(false);
                _hasSecondJump = false;
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && (_grounded ? _groundDashRemainingCooldown <= 0 : _hasAirDash))
        {
            UnfreezeDirection();
            StartDashing();
            StartCoroutine(DashCoroutine());
            if (_grounded)
                _groundDashRemainingCooldown = _groundDashCooldown;
            else
                _hasAirDash = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _time = 0;


        _m = GetComponent<CircularMovement>();
        _rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.right * 15;
        _direction = 1;
        _hasSecondJump = true;

        _airAcc = _baseAirAcc;

        _coins = 0;

        _health = _maxHealth;

        _ingameUI = uiIngameObj.GetComponent<UIIngame>();

        _invisFrames = false;

        _isDead = false;

        _freezeDirection = false;
    }


    void FixedUpdate()
    {
        if (transform.position.y < -_deathDepth)
        {
            death();
            return;
        }
        if (!_dashing)
        {
            var velocity = _m.ToLocal(_rb.velocity);
            if (!_grounded)
            {

                float targetSpeed = _speed * _movement * _m.GetDirection();
                if (Mathf.Abs(targetSpeed - velocity.x) <= _airAcc * Time.fixedDeltaTime)
                {
                    velocity.x = targetSpeed;
                }
                else
                {
                    float factor = 1;
                    if (targetSpeed < velocity.x)
                        factor = -1;
                    velocity.x += _airAcc * Time.fixedDeltaTime * factor;
                }

            }
            /**/
            else
            {
                velocity.x = _speed * _movement * _m.GetDirection();
            }

            if (_touchingWall)
            {
                velocity.y = Mathf.Max(velocity.y, -_wallSlideSpeed);
            }/**/
            velocity.x = Mathf.Clamp(velocity.x, -_speed, _speed);
            velocity.y = Mathf.Clamp(velocity.y, -30.0f, 30.0f);
            _rb.velocity = _m.FromLocal(velocity);

            //_rb.AddForce(_m.fromLocal(2.0f * _movement * _moveVector), ForceMode.VelocityChange);
        }
        else
        {
            _currentDashSpeed -= 1.0f;
            var velocity = _m.ToLocal(_rb.velocity);
            velocity.x = _currentDashSpeed;
            velocity.y = 0;
            _rb.velocity = _m.FromLocal(velocity);
        }
        if (_groundDashRemainingCooldown >= 0)
            _groundDashRemainingCooldown -= Time.fixedDeltaTime;
        if (_remainingCoyoteTime >= 0)
            _remainingCoyoteTime -= Time.fixedDeltaTime;

        SetGrounded(false);


    }

    private void Update()
    {
        if (!_isDead)
            _time = _time + Time.deltaTime;
        string _format = timeFormat();
        _ingameUI.SetTimer(_format);

    }

    private string timeFormat()
    {
        int minutes = Mathf.FloorToInt(_time / 60F);
        int seconds = Mathf.FloorToInt(_time - minutes * 60);
        int milliseconds = Mathf.FloorToInt(_time * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        string format = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        return format;
    }



    public void SetGrounded(bool grounded)
    {
        this._grounded = grounded;
        if (_grounded == true)
        {
            this._hasSecondJump = true;
            this._hasAirDash = true;
            _remainingCoyoteTime = _coyoteTime;
        }
    }

    public void UnfreezeDirection()
    {
        _freezeDirection = false;
        if (_movement != 0)
        {
            _direction = _movement;
            _m.SetDirection(_direction);
        }
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

    public IEnumerator WallJumpCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        _airAcc = 0.2f * _baseAirAcc;
        yield return new WaitForSeconds(0.25f);
        _airAcc = 0.5f * _baseAirAcc;
        UnfreezeDirection();
        yield return new WaitForSeconds(0.1f);
        _airAcc = 1.0f * _baseAirAcc;
    }

    public IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        EndDashing();
    }

    public IEnumerator InvisFramesCoroutine()
    {
        _invisFrames = true;
        yield return new WaitForSeconds(_invisFramesDuration);
        _invisFrames = false;
    }

    public void StartDashing()
    {
        _currentDashSpeed = _dashSpeed;
        var velocity = _m.ToLocal(_rb.velocity);
        velocity.x = _currentDashSpeed;
        velocity.y = 0;
        _rb.velocity = _m.FromLocal(velocity);
        _dashing = true;
        _rb.useGravity = false;
    }

    public void EndDashing()
    {
        _dashing = false;
        _rb.useGravity = true;
    }

    public void addCoin()
    {
        _ingameUI.SetCollectableText("" + (++_coins));
    }

    public void damage()
    {
        if (!_invisFrames)
        {
            StartCoroutine(InvisFramesCoroutine());
            _ingameUI.SetLives(--_health);
            if (_health <= 0)
                death();
        }
    }

    public void death()
    {
        if (!_isDead)
        {
            GetComponent<CenterMouse>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameObject o = Instantiate(endScreenPrefab);
            _isDead = true;
            UIEnd e = o.GetComponent<UIEnd>();
            e.SetCollectedCoinsText(_coins + "");
            e.SetTimeText(_time);
            LevelSettings settings = null;
            foreach (var x in PreviewSettings.levelSettings)
                if (x.levelnumber == levelnumber)
                    settings = x;
            if (settings.fastestTime == -1)
                e.SetRecordTimeText(-1);
            else
                e.SetRecordTimeText(settings.fastestTime);


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && !_triggered)
        {
            _isDead = true;
            GetComponent<CenterMouse>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            this.GetComponent<Player>().enabled = false;
            this.GetComponent<Rigidbody>().isKinematic = true;
            GameObject o = Instantiate(winScreenPrefab);
            _triggered = true;

            UIEnd e = o.GetComponent<UIEnd>();
            e.SetCollectedCoinsText(_coins + "/" + _ingameUI.collectables);
            e.SetTimeText(_time);

            LevelSettings settings = null;
            foreach (var x in PreviewSettings.levelSettings)
                if (x.levelnumber == levelnumber)
                    settings = x;

            if (settings.fastestTime == -1 || settings.fastestTime > _time)
            {
                settings.fastestTime = _time;
            }
            if (settings.collectablesCollected == -1 || settings.collectablesCollected < _coins)
            {
                settings.collectablesCollected = _coins;
            }

            e.SetRecordTimeText(settings.fastestTime);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != gameObject && collision.gameObject.CompareTag("terrain"))
        {
            //Debug.Log(collision.impulse.ToString());
            Vector3 vec;
            bool validVectorFound = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                vec = collision.GetContact(i).point - transform.position;
                if (-vec.y > new Vector2(vec.x, vec.z).magnitude * 3)
                {
                    validVectorFound = true;
                    break;
                }

            }
            if (validVectorFound)
            {
                SetGrounded(true);
                //Debug.Log("grounded");
                /*
                RaycastHit hit;
                if (_grounded && Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    Vector2 normal2D = GetComponent<CircularMovement>().ToLocal(hit.normal);
                    Debug.Log(normal2D.ToString());
                    _moveVector = new Vector2(normal2D.y, -normal2D.x);
                }
                else
                {
                    _moveVector = new Vector2(1.0f, 0.0f);
                }
                /**/
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject != gameObject && collision.gameObject.CompareTag("terrain"))
        {
            //Debug.Log(collision.impulse.ToString());
            Vector3 vec;
            bool validVectorFound = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                vec = collision.GetContact(i).point - transform.position;
                if (-vec.y > new Vector2(vec.x, vec.z).magnitude * 3)
                {
                    validVectorFound = true;
                    break;
                }

            }
            if (validVectorFound)
            {
                SetGrounded(true);
            }
        }
    }

    public void setPaused(bool paused)
    {
        _isPaused = paused;
    }
}
