using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentScript : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField] private Transform[] waypoints;

    private int _current = 0;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.destination = waypoints[_current].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance < .5f)
        {
            NextWaypoint();
        }
    }

    private void NextWaypoint()
    {
        _current = (_current + 1) % waypoints.Length;
        _agent.destination = waypoints[_current].position;
    }
}
