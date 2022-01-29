using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Rigidbody attached to the player
    /// </summary>
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private GameObject Body;
    [SerializeField] private Transform cameraTransform;
    
    /// <summary>
    /// Script to check for grounded state
    /// </summary>
    [SerializeField] private GroundDetection groundCheck;

    /// <summary>
    /// Receive inputs from this script to execute
    /// </summary>
    [SerializeField] private InputManager inputManager;
    
    /// <summary>
    /// Whether input will be read to move the player.
    /// </summary>
    private bool canMove;

    /// <summary>
    /// Speed player moves in any direction.
    /// </summary>
    public float accelRateGround, accelRateAir;

    [SerializeField] private float velocityGroundLimit, velocityAirLimit;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (inputManager.GetIsJumping() && !groundCheck.GetIsGrounded())
        {
            Rb.velocity = new Vector3(Rb.velocity.x, 100, Rb.velocity.z);
        }
    }

    void FixedUpdate()
    {
        RotateBodyWithCamera();
        ConvertInputsToMovementDir();
    }

    private void Setup() 
    {
    }

    /// <summary>
    /// Convert movement inputs from Vector2 to Vector3
    /// </summary>
    private void ConvertInputsToMovementDir()
    {
        var inputs = inputManager.GetWASD();
        if (inputs == Vector2.zero && groundCheck.GetIsGrounded())
        {
            Rb.velocity = Vector3.zero;
            return;
        }

        Vector3 moveDir;
        if (groundCheck.GetIsGrounded()) 
            moveDir = new Vector3(inputs.x, 0, inputs.y);
        else
            moveDir = new Vector3(inputs.x, Rb.velocity.y, inputs.y);


        moveDir = Body.transform.TransformDirection(moveDir);
        
        MoveInDirection(moveDir.normalized); 
    }
    /// <summary>
    /// Accelerate player over time in direction they are aiming
    /// </summary>
    /// <param name="dir"></param>
    private void MoveInDirection(Vector3 dir)
    {
        Rb.velocity += dir * (accelRateGround * Time.deltaTime);
        Rb.velocity = VelocityChecks();
        print(dir.ToString());
    }

    private Vector3 VelocityChecks()
    {
        var velCheck = Rb.velocity;
        
        if (velCheck.x > velocityGroundLimit && groundCheck.GetIsGrounded())
            velCheck = new Vector3(velocityGroundLimit, velCheck.y, velCheck.z);
        else if (velCheck.x > velocityAirLimit && !groundCheck.GetIsGrounded())
            velCheck = new Vector3(velocityAirLimit, velCheck.y, velCheck.z);
        
        if (velCheck.x < -velocityGroundLimit && groundCheck.GetIsGrounded())
            velCheck = new Vector3(-velocityGroundLimit, velCheck.y, velCheck.z);
        else if (velCheck.x < -velocityAirLimit && !groundCheck.GetIsGrounded())
            velCheck = new Vector3(-velocityAirLimit, velCheck.y, velCheck.z);

        if (velCheck.y > 5)  
            velCheck = new Vector3(velCheck.x, 5, velCheck.z);
        if (velCheck.y < -5)
            velCheck = new Vector3(velCheck.x, -15, velCheck.z);
                
        if (velCheck.z > velocityGroundLimit && groundCheck.GetIsGrounded())
            velCheck = new Vector3(velCheck.x, velCheck.y, velocityGroundLimit);
        else if (velCheck.z > velocityAirLimit && !groundCheck.GetIsGrounded())
            velCheck = new Vector3(velCheck.x, velCheck.y, velocityAirLimit);
        
        if (velCheck.z < -velocityGroundLimit && groundCheck.GetIsGrounded())
            velCheck = new Vector3(velCheck.x, velCheck.y, -velocityGroundLimit);
        else if (velCheck.z < -velocityAirLimit && !groundCheck.GetIsGrounded())
            velCheck = new Vector3(velCheck.x, velCheck.y, -velocityAirLimit);

        return velCheck;
    }
    /// <summary>
    /// Rotate the player's body to follow the Y of the Camera.
    /// </summary>
    private void RotateBodyWithCamera()
    {
        var rotation = Body.transform.rotation;
        rotation = new Quaternion(rotation.x, cameraTransform.forward.y, rotation.z, rotation.w);
        Body.transform.rotation = rotation;
    }
}
