using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{

    private float _radius;
    private Rigidbody _rb;
    private bool _frontFacing;

    private void Start()
    {
        
        _rb = GetComponent<Rigidbody>();

        UpdateRadius();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, GetDegrees(), transform.eulerAngles.z);

        //Debug.Log(Mathf.Atan2(0, 1));
        //Debug.Log(Mathf.Atan2(1, 0));
        //Debug.Log(Mathf.Atan2(0, -1));
        //Debug.Log(Mathf.Atan2(-1, 0));
    }

    private void FixedUpdate()
    {
        PullOnCircle();
        if(_rb != null)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, GetDegrees() + (_frontFacing ? 0 : 180), transform.eulerAngles.z);
        }
        else
        {
            var velocity = ToLocal(_rb.velocity);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, GetDegrees() + (_frontFacing ? 0 : 180), transform.eulerAngles.z);
            _rb.velocity = FromLocal(velocity);
        }
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

    public Vector2 ToLocal(Vector3 vec)
    {
        Vector3 temp = transform.InverseTransformDirection(vec);
        return new Vector2(temp.z, temp.y);
    }

    public Vector3 FromLocal(Vector2 vec)
    {
        return transform.TransformDirection(new Vector3(0, vec.y, vec.x));
    }

    public bool IsFrontFacing()
    {
        return _frontFacing;
    }

    public int GetDirection()
    {
        return _frontFacing ? 1 : -1;
    }

    public void SetDirection(int dir)
    {
        SetFrontFacing(dir == 1 ? true : false);
    }

    public void SetFrontFacing(bool frontFacing)
    {
       transform.eulerAngles = new Vector3(transform.eulerAngles.x, GetDegrees() + (_frontFacing ? 0 : 180), transform.eulerAngles.z);
        _frontFacing = frontFacing;
    }
}
