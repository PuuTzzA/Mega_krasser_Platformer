using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MountainGoat : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private float detectionRange;

    [Header("Patrol Settings")] [SerializeField]
    private Transform[] patrolPoints;

    [SerializeField] private float patrolMaxSpeed;
    [SerializeField] private float patrolAcceleration;
    [SerializeField] private float rotationSpeed;

    [Header("Attack Settings")] [SerializeField]
    private float attackWindup;

    [SerializeField] private float attackStrenght;


    enum State
    {
        PATROLE,
        ATTACKING,
    }

    private State _state = State.PATROLE;
    private Rigidbody _rigidbody;
    private int _currentPatrolPoint;
    private Quaternion _goalRotation;
    private float _attackTime;
    private bool _attackFp;
    private bool _attackBefore;
    private bool _attackImpulseFp;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_state)
        {
            case State.PATROLE:
                Patrol();
                break;
            case State.ATTACKING:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, _goalRotation, Time.fixedDeltaTime * rotationSpeed);

        // Check if Player is in Detection Range and in front of the goat 
        if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange &&
            Vector3.Dot(transform.forward, player.transform.position) < 0)
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
        _attackBefore = _attackFp;
    }

    private void Patrol()
    {
        Vector3 current = transform.position;
        Vector3 goal = patrolPoints[_currentPatrolPoint].position;

        Vector3 dir = (goal - current).normalized;

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
        Quaternion tempRotationStart = transform.rotation;

        transform.LookAt(player.transform.position);
        _goalRotation = transform.rotation;
        _goalRotation *= Quaternion.Euler(0, 180, 0);
        _goalRotation = Quaternion.Euler(0, _goalRotation.eulerAngles.y, 0);

        transform.rotation = tempRotationStart;
        _attackTime = 0;
        _attackImpulseFp = true;
    }

    private void Attack()
    {
        if (_attackTime < attackWindup)
        {
            _attackTime += Time.fixedDeltaTime;
            return;
        }
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
}