using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class NickNocksCharacterController : MonoBehaviour
{
    // Start is called before the first frame upda
    [SerializeField]
    private Rigidbody playerRigidbody;

    public GameObject camHolder;

    private Vector2 move, look;

    [SerializeField]
    private float speed, maxForce, sensitivity, jumpForce;

    private float lookRotation;

    public bool grounded;



    [SerializeField] private Transform spawnPoint;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Spawn();
    }

    private void Awake()
    {
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Move()
    {
        Vector3 currentVelocity = playerRigidbody.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        Vector3.ClampMagnitude(velocityChange, maxForce);

        playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    private void Look()
    {
        //Turn
        transform.Rotate(Vector3.up * look.x * sensitivity);

        //Look
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    private void Jump(){
        Vector3 jumpForces = Vector3.zero;

        if(grounded){
            jumpForces = Vector3.up * jumpForce;
        }

        playerRigidbody.AddForce(jumpForces, ForceMode.Impulse);
    }

    public void setGrounded(bool b){
        grounded = b;
    }

    private void Spawn() { transform.position = spawnPoint.position; }
}