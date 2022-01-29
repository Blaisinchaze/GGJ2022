using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]

    //  ====    External Scripts
    
    [SerializeField] private GroundDetection groundCheck;
    [SerializeField] private InputManager inputManager;
    
    //  ====    Components    
    
    /// <summary>
    /// Rigidbody attached to the player
    /// </summary>
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private GameObject Body;
    [SerializeField] private Transform Head;
    [SerializeField] private Transform cameraTransform;
    
    // =====    Camera Stuff
    
    private float xMouseSensitivity = 30.0f;
    private float yMouseSensitivity = 30.0f;
    private float rotX = 0.0f;
    private float rotY = 0.0f;
     
    [Header("Movement values")]
    //  ====    Movement
    
    public float moveSpeed;
    
    /// <summary>
    /// Direction of player movement.
    /// </summary>
    private Vector3 movementDirection = Vector3.zero;

    /// <summary>
    /// The height of the player's jump.
    /// </summary>
    [SerializeField] private float jumpHeight;
    
    
    private void Setup()
    {
        inputManager.onJumpPressed += DoJump;
    }
    void Start()
    {
        Setup();
    }

    void Update()
    {
        movementDirection = ConvertInputsToMovementDir();
    }

    private void FixedUpdate()
    {
        if (movementDirection != Vector3.zero)
            ExecuteMovement();
        cameraTransform.position = Head.position;

    }

    private void LateUpdate()
    {
        //  Move the camera along with the head
        RotateCamera();
    }

    /// <summary>
    /// Execute jump, only called by delegate
    /// </summary>
    private void DoJump()
    {
        if (!groundCheck.GetIsGrounded()) return;
        Rb.velocity = new Vector3(Rb.velocity.x, jumpHeight, Rb.velocity.z);  
    }
    
    /// <summary>
    /// Convert movement inputs from Vector2 to Vector3
    /// </summary>
    private Vector3 ConvertInputsToMovementDir()
    {
        var inputs = inputManager.GetWASD().normalized;
        
        //  If no input is pressed, exit and don't move player
        if (inputs == Vector2.zero && groundCheck.GetIsGrounded())
        {
            Rb.velocity = Vector3.zero;
            return Vector3.zero;
        }
        
        Vector3 moveDir = new Vector3(inputs.x, 0, inputs.y);
        
        //  Localise the direction vector
        moveDir = Body.transform.TransformDirection(moveDir);
        
        //  Add back the Y velocity so jumping isn't affected
        moveDir = new Vector3(moveDir.x, Rb.velocity.y, moveDir.z);
        
        
        //  Execute movement
        return moveDir;
    }
    
    /// <summary>
    /// Accelerate player over time in direction they are aiming
    /// </summary>
    /// <param name="dir"></param>
    private void ExecuteMovement()
    {
        var finalSpeed = moveSpeed * 10;
        
        if (inputManager.GetSprinting())
            Rb.velocity = new Vector3(
                movementDirection.x * (finalSpeed * 1.5f * Time.deltaTime),
                Rb.velocity.y,
                movementDirection.z * (finalSpeed * 1.5f * Time.deltaTime)  );
        else
            Rb.velocity = new Vector3(
                movementDirection.x * (finalSpeed * Time.deltaTime),
                Rb.velocity.y,
                movementDirection.z * (finalSpeed * Time.deltaTime)  );
    }


    // private Vector3 VelocityChecks()
    // {
    //     var velCheck = Rb.velocity;
    //     
    //     if (velCheck.x > velocityGroundLimit && groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velocityGroundLimit, velCheck.y, velCheck.z);
    //     else if (velCheck.x > velocityAirLimit && !groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velocityAirLimit, velCheck.y, velCheck.z);
    //     
    //     if (velCheck.x < -velocityGroundLimit && groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(-velocityGroundLimit, velCheck.y, velCheck.z);
    //     else if (velCheck.x < -velocityAirLimit && !groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(-velocityAirLimit, velCheck.y, velCheck.z);
    //
    //     if (velCheck.z > velocityGroundLimit && groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velCheck.x, velCheck.y, velocityGroundLimit);
    //     else if (velCheck.z > velocityAirLimit && !groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velCheck.x, velCheck.y, velocityAirLimit);
    //     
    //     if (velCheck.z < -velocityGroundLimit && groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velCheck.x, velCheck.y, -velocityGroundLimit);
    //     else if (velCheck.z < -velocityAirLimit && !groundCheck.GetIsGrounded())
    //         velCheck = new Vector3(velCheck.x, velCheck.y, -velocityAirLimit);
    //
    //     return velCheck;
    // }
    

    private void RotateCamera()
    {
        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        // Clamp rotation
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;

        // Rotates the collider
        Body.transform.rotation = Quaternion.Euler(0, rotY, 0); 
        // Rotates the camera
        cameraTransform.rotation = Quaternion.Euler(rotX, rotY, 0); 
    }
}
