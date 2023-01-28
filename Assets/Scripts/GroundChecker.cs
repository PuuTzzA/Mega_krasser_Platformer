using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        Debug.Log("setting false");
        player.SetGrounded(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player.gameObject)
        {
            player.SetGrounded(true);
            RaycastHit hit;
            if (player.IsGrounded() && Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                Vector2 normal2D = player.GetComponent<CircularMovement>().ToLocal(hit.normal);
                Debug.Log(normal2D.ToString());
                player.SetMoveVector(new Vector2(normal2D.y, -normal2D.x));
            }
            else
            {
                player.SetMoveVector(new Vector2(1.0f, 0.0f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != player.gameObject)
        {
            player.SetGrounded(false);
            player.SetMoveVector(new Vector2(1.0f, 0.0f));
        }
    }
    
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject != player.gameObject)
        {
            Debug.Log("stayed, so true");
            player.SetGrounded(true);
            RaycastHit hit;
            if (player.IsGrounded() && Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                Vector2 normal2D = player.GetComponent<CircularMovement>().ToLocal(hit.normal);
                Debug.Log(normal2D.ToString());
                player.SetMoveVector(new Vector2(normal2D.y, -normal2D.x));
            }
            else
            {
                player.SetMoveVector(new Vector2(1.0f, 0.0f));
            }
        }
    }

    public float GetVerticalSpeedDiff(GameObject other)
    {
        float ySpeed = player.GetComponent<Rigidbody>().velocity.y;
        var rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            return ySpeed - rb.velocity.y;
        }
        return ySpeed;
    }/**/
}
