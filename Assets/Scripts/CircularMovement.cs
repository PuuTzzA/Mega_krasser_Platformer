using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement
{

    public Transform refPoint;
    public Transform mutablePoint;
    public float radius;

    public CircularMovement(GameObject mutablePoint, Transform parent)
    {
        this.mutablePoint = mutablePoint.transform;
        this.refPoint = new GameObject("empty").transform;

        this.refPoint.SetParent(parent);
        this.mutablePoint.SetParent(this.refPoint);

        this.radius = new Vector2(this.mutablePoint.position.x, this.mutablePoint.position.z).magnitude;

        Debug.Log(radius);
    }

    public Vector2 get()
    {
        return new Vector2(-refPoint.eulerAngles.y / 180.0f * Mathf.PI, refPoint.position.y);
    }

    public void set(Vector2 pos)
    {
        Debug.Log(pos.ToString());
        refPoint.position.Set(refPoint.position.x, pos.y, refPoint.position.z);
        refPoint.eulerAngles = new Vector3(refPoint.eulerAngles.x, -pos.x / Mathf.PI * 180.0f, refPoint.eulerAngles.z);
        Vector2 vec = new Vector2(mutablePoint.position.x, mutablePoint.position.z);
        mutablePoint.position = vec.normalized * radius;
        Debug.Log(refPoint.eulerAngles.y);
    }
}
