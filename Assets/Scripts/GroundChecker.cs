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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player.gameObject)
            player.SetGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != player.gameObject)
            player.SetGrounded(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != player.gameObject)
            player.SetGrounded(true);
    }
}
