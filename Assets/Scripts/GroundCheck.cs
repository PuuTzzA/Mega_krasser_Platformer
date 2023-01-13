using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public NickNocksCharacterController playerController;
   private void OnTriggerEnter(Collider other) {
    if(other.gameObject == playerController.gameObject){
        return;
    }
    playerController.setGrounded(true);
   }

    private void OnTriggerExit(Collider other) {
    if(other.gameObject == playerController.gameObject){
        return;
    }
    playerController.setGrounded(false);
   }

    private void OnTriggerStay(Collider other) {
    if(other.gameObject == playerController.gameObject){
        return;
    }
    playerController.setGrounded(true);
   }
}
