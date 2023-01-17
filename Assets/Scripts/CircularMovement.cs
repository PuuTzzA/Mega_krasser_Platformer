using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    private Transform _prevTransform;

    private float _radius;
    private Rigidbody _rb;

    private void Start()
    {
        _prevTransform = transform;
        _rb = GetComponent<Rigidbody>();

        UpdateRadius();

        Debug.Log(Mathf.Atan2(0, 1));
        Debug.Log(Mathf.Atan2(1, 0));
        Debug.Log(Mathf.Atan2(0, -1));
        Debug.Log(Mathf.Atan2(-1, 0));
    }

    private void FixedUpdate()
    {
        PullOnCircle();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, GetDegrees(), transform.eulerAngles.z);
        Debug.Log(GetDegrees());
        //Debug.Log(_rb.velocity.ToString());
    }

    public void UpdateRadius()
    {
        _radius = GetXZPosition().magnitude;
    }

    public void SetRadius(float radius)
    {
        this._radius = radius;
    }

    public float GetRadius()
    {
        return _radius;
    }

    public void PullOnCircle()
    {
        SetXZPosition(GetXZPosition().normalized * _radius);
    }

    public Vector2 GetXZPosition()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }

    public void SetXZPosition(Vector2 vec)
    {
        transform.position = new Vector3(vec.x, transform.position.y, vec.y);
    }

    public float GetHeight()
    {
        return transform.position.y;
    }

    public void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    public float GetDegrees()
    {
        return -Mathf.Atan2(GetXZPosition().y, GetXZPosition().x) * 180 / Mathf.PI;
    }

    public void SetDegrees(float degrees)
    {
        SetXZPosition(new Vector2(Mathf.Cos(degrees * Mathf.PI / 180.0f), - Mathf.Sin(degrees * Mathf.PI / 180.0f)));
    }

    public Vector2 toLocal(Vector3 vec)
    {
        Vector3 temp = transform.InverseTransformDirection(vec);
        return new Vector2(temp.z, temp.y);
    }

    public Vector3 fromLocal(Vector2 vec)
    {
        return transform.TransformDirection(new Vector3(0, vec.y, vec.x));
    }
}
