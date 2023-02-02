using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MountainGoat : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float detectionRange;

    [Header("Patrol Settings")] [SerializeField]
    private Transform[] patrolPoints;

    [SerializeField] private float patrolMaxSpeed;
    [SerializeField] private float patrolAcceleration;
    [SerializeField] private float rotationSpeed;

    [Header("Attack Settings")] [SerializeField]
    private float attackWindup;

    [SerializeField] private float attackStrenght;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject angryIndicator;

    [Header("Stun Settings")] [SerializeField]
    private float stunDuration;

    [SerializeField] private GameObject indicator;
    [SerializeField] private Transform stunPosition;

    public enum State
    {
        PATROLE,
        ATTACKING,
        STUNNED
    }

    public State _state = State.PATROLE;
    private Rigidbody _rigidbody;
    private int _currentPatrolPoint;
    private Quaternion _goalRotation;
    private float _attackTime;
    private bool _attackFp;
    private bool _attackBefore;
    private bool _attackImpulseFp;
    private GameObject _stunIndicator;
    private float _stunTime;

    private float _timeSinceNotMoving;
    private Vector3 _oldPosition;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Destroy the goat if it wants to commit suicide
        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }

        if (Vector3.Distance(transform.position, _oldPosition) < .05f && _state != State.STUNNED)
        {
            _timeSinceNotMoving += Time.fixedDeltaTime;
            if (_timeSinceNotMoving > 3.5f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _timeSinceNotMoving = 0;
        }
        _oldPosition = transform.position;

        switch (_state)
        {
            case State.PATROLE:
                Destroy(_stunIndicator);
                Patrol();
                break;
            case State.ATTACKING:
                RotateToPlayer();
                break;
            case State.STUNNED:
                _stunTime += Time.fixedDeltaTime;
                if (_stunTime > stunDuration)
                {
                    _state = State.PATROLE;
                    _attackBefore = false;
                }

                break;
        }

        if (_state == State.STUNNED)
        {
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, _goalRotation, Time.fixedDeltaTime * rotationSpeed);

        // Check if Player is in Detection Range and in front of the goat 
        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange &&
            Vector3.Dot(transform.forward.normalized, player.transform.position.normalized) < 0 &&
            Mathf.Abs(player.transform.position.y - transform.position.y) < 5f)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, layerMask);
            if (hit.collider.gameObject == player)
            {
                _attackFp = true;
                if (!_attackBefore && _attackFp)
                {
                    _state = State.ATTACKING;
                    StartAttack();
                }

                Attack();
            }
            else
            {
                _attackFp = false;
                _state = State.PATROLE;
            }
        }
        else
        {
            _attackFp = false;
            _state = State.PATROLE;
        }

        _attackBefore = _attackFp;
    }

    private void Patrol()
    {
        Vector3 current = transform.position;
        Vector3 goal = patrolPoints[_currentPatrolPoint].position;

        Vector3 dir = (goal - current).normalized;
        dir = new Vector3(dir.x, 0, dir.z).normalized;

        Quaternion tempRotationStart = transform.rotation;

        transform.LookAt(goal);
        _goalRotation = transform.rotation;
        _goalRotation *= Quaternion.Euler(0, 180, 0);
        _goalRotation = Quaternion.Euler(0, _goalRotation.eulerAngles.y, 0);

        transform.rotation = tempRotationStart;

        if (Vector3.Dot(_rigidbody.velocity, dir) < patrolMaxSpeed)
        {
            _rigidbody.AddForce(dir * patrolAcceleration);
        }

        if (Vector3.Distance(current, goal) < 0.5f)
        {
            _currentPatrolPoint = (_currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }

    private void StartAttack()
    {
        RotateToPlayer();
        _attackTime = 0;
        _attackImpulseFp = true;
        Destroy(_stunIndicator);
        _stunIndicator = (GameObject)Instantiate(angryIndicator, stunPosition.position, Quaternion.identity);
        _stunIndicator.transform.parent = transform;
    }

    private void RotateToPlayer()
    {
        Quaternion tempRotationStart = transform.rotation;

        transform.LookAt(player.transform.position);
        _goalRotation = transform.rotation;
        _goalRotation *= Quaternion.Euler(0, 180, 0);
        _goalRotation = Quaternion.Euler(0, _goalRotation.eulerAngles.y, 0);

        transform.rotation = tempRotationStart;
    }

    private void Attack()
    {
        if (_attackTime < attackWindup)
        {
            _attackTime += Time.fixedDeltaTime;
            return;
        }

        Destroy(_stunIndicator);

        if (_attackImpulseFp)
        {
            _attackImpulseFp = false;
            Vector3 force = (player.transform.position - transform.position).normalized;
            _rigidbody.AddForce(force * attackStrenght, ForceMode.Impulse);
            return;
        }

        if (_rigidbody.velocity.magnitude < 0.5f)
        {
            StartAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Snowball"))
        {
            _state = State.STUNNED;
            Destroy(_stunIndicator);
            _stunIndicator = (GameObject)Instantiate(indicator, stunPosition.position, Quaternion.identity);
            _stunIndicator.transform.parent = transform;
            _stunTime = 0;
        }
    }
}