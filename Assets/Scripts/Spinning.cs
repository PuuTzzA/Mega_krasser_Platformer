using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{

    public float angularSpeed = 180.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var angles = transform.eulerAngles;
        angles.y += angularSpeed * Time.deltaTime;
        transform.eulerAngles = angles;
    }
}
