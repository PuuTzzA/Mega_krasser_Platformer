using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulsisMovement : MonoBehaviour
{

    private CircularMovement m;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        m = new CircularMovement(gameObject, null);

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vector2 = m.get();

        //Debug.Log(vector2.ToString());

        vector2.x += Input.GetAxis("Horizontal") * 0.01f;

        if (Input.GetKeyDown(KeyCode.Space)) {
            var vel = rb.velocity;
            vel.y = 5;
            rb.velocity = vel;
        }

        m.set(vector2);
        
    }
}
