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
        m = GetComponent<CircularMovement>();
        rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.right * 15;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 velocity;
        //velocity.x = Input.GetAxisRaw("Horizontal") * 15;
        //if(Input.GetKeyDown(KeyCode.Space))
        //    velocity.y = 10;
        //else
        //    velocity.y = rb.velocity.y;
        //
        //rb.velocity = m.fromLocal(velocity);
    }
}
