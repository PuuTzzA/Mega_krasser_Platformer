using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class TentaclePlantTentacle : MonoBehaviour
{
    [Header("General Settings")] public GameObject player;

    public float detectionRadius;

    [SerializeField] private int length;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothSpeedHead;

    [Header("Set over Tentacle Plant, not here!!!")] 
    public float grappleStrength;
    public float breakFreeDistance;
    public float maxLength;

    private LineRenderer _lineRenderer;
    private Vector3[] _segmentPositions;
    private Vector3[] _segmentVelocities;
    private bool[] _isGrappling;

    private Collider _playerCollider;
    private Rigidbody _playerRigidbody;
    private Vector3 _idleGoal;
    private Vector3 _playerPosOld;

    private enum State
    {
        IDLE,
        GRAPPLING,
    }

    private State _state = State.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _playerCollider = player.GetComponent<CapsuleCollider>();
        _playerRigidbody = player.GetComponent<Rigidbody>();

        _segmentPositions = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            _segmentPositions[i] = transform.position;
        }

        _segmentVelocities = new Vector3[length];
        _isGrappling = new bool[length];

        _lineRenderer.positionCount = length;
        UpdateIdlePosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_state)
        {
            case State.IDLE:
                UpdatePositionsIdle();
                break;
            case State.GRAPPLING:
                Vector3 posPlayer = player.transform.position;
                _playerRigidbody.AddForce(
                    (posPlayer - transform.position).normalized * -grappleStrength);
                UpdatePositionsGrappling(posPlayer);
                _playerPosOld = posPlayer;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        bool chasing = false;
        for (int i = 0; i < length; i++)
        {
            if (IsInsideCollider(_segmentPositions[i]) && Vector3.Distance(transform.position, _segmentPositions[i]) <= maxLength)
            {
                _state = State.GRAPPLING;
                chasing = true;
                _isGrappling[i] = true;
            }
            else
            {
                _isGrappling[i] = false;
            }
        }

        if (!chasing)
        {
            _state = State.IDLE;
        }
    }

    private void UpdatePositionsIdle()
    {
        _segmentPositions[0] = transform.position;
        _segmentPositions[length - 1] = Vector3.SmoothDamp(
            _segmentPositions[length - 1], _idleGoal,
            ref _segmentVelocities[length - 1], smoothSpeedHead * Time.fixedDeltaTime);

        Vector3[] segmentPositionsSmooth = (Vector3[])_segmentPositions.Clone();

        for (int i = 1; i <= length - 2; i++)
        {
            Vector3 goalPosition = (_segmentPositions[i - 1] + _segmentPositions[i + 1]) / 2;

            segmentPositionsSmooth[i] = Vector3.SmoothDamp(_segmentPositions[i], goalPosition,
                ref _segmentVelocities[i], smoothSpeed * Time.fixedDeltaTime);
        }

        _segmentPositions = (Vector3[])segmentPositionsSmooth.Clone();
        _lineRenderer.SetPositions(segmentPositionsSmooth);

        if (Vector3.Distance(_segmentPositions[length - 1], _idleGoal) < 0.1f)
        {
            UpdateIdlePosition();
        }
    }

    private void UpdatePositionsGrappling(Vector3 playerPos)
    {
        _segmentPositions[0] = transform.position;
        Vector3 playerPosDelta = _playerPosOld - playerPos;

        Vector3[] segmentPositionsSmooth = (Vector3[])_segmentPositions.Clone();
        int highestGrapplePoint = 0;
        int lowestGrapplePoint = length;

        for (int i = 1; i < length; i++)
        {
            if (!_isGrappling[i]) continue;
            if (i < lowestGrapplePoint)
            {
                lowestGrapplePoint = i;
            }
            else if (i > highestGrapplePoint)
            {
                highestGrapplePoint = i;
            }
        }

        for (int i = 1; i < length; i++)
        {
            if (_isGrappling[i])
            {
                playerPosDelta = Vector3.ClampMagnitude(playerPosDelta, breakFreeDistance);
                segmentPositionsSmooth[i] = _segmentPositions[i] - playerPosDelta;
                continue;
            }

            if (i < lowestGrapplePoint)
            {
                segmentPositionsSmooth[i] = Vector3.SmoothDamp(_segmentPositions[i], transform.position,
                    ref _segmentVelocities[i], smoothSpeed * Time.fixedDeltaTime);
                continue;
            }

            segmentPositionsSmooth[i] = Vector3.SmoothDamp(_segmentPositions[i], playerPos,
                ref _segmentVelocities[i], smoothSpeedHead * Time.fixedDeltaTime);
        }

        _segmentPositions = (Vector3[])segmentPositionsSmooth.Clone();
        _lineRenderer.SetPositions(segmentPositionsSmooth);
    }

    private bool IsInsideCollider(Vector3 pos)
    {
        return _playerCollider.ClosestPoint(pos) == pos;
    }

    private void UpdateIdlePosition()
    {
        do
        {
            _idleGoal = new Vector3(UnityEngine.Random.Range(-detectionRadius, detectionRadius),
                UnityEngine.Random.Range(-detectionRadius, detectionRadius),
                UnityEngine.Random.Range(-detectionRadius, detectionRadius));
        } while (Vector3.Dot(transform.up, _idleGoal) < 0 || _idleGoal.magnitude > detectionRadius);

        _idleGoal += transform.position;
    }
}