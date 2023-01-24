using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGoat : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float detectionRange;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolMaxSpeed;
    [SerializeField] private float patrolAcceleration;

    enum State
    {
        PATROLE,
        ATTACKING,
    }

    private State _state = State.PATROLE;
    private Rigidbody _rigidbody;
    private int _currentPatrolPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
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
    }

    private void Patrol()
    {
        Vector3 current = transform.position;
        Vector3 goal = patrolPoints[_currentPatrolPoint].position;

        Vector3 dir = (goal - current).normalized;
        
        transform.LookAt(goal);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        
        if (Vector3.Dot(_rigidbody.velocity, dir) < patrolMaxSpeed)
        {
            _rigidbody.AddForce(dir * patrolAcceleration);
        }        
        
        if (Vector3.Distance(current, goal) < 0.5f)
        {
            _currentPatrolPoint = (_currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }
}
