using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Rigidbody attached to the player
    /// </summary>
    [SerializeField] private Rigidbody Rb;
    public enum MovementState
    {
        Neutral = 0,
        Moving,
        Jumping
    }

    /// <summary>
    /// Whether input will be read to move the player.
    /// </summary>
    private bool canMove;

    /// <summary>
    /// Speed player moves in any direction.
    /// </summary>
    public float MoveSpeed;
    
    
    void Start()
    {
        Setup();
    }

    void Update()
    {
        HandleInputs();
    }

    private void Setup()
    {
        Rb = GetComponent<Rigidbody>();
    }
    private void HandleInputs()
    {
        //  Jumping
        
        //  Movement
        if (!canMove) return;
        if (Input.GetKeyDown("W"))
            MoveInDirection(Vector3.forward);
        if (Input.GetKeyDown("S"))
            MoveInDirection(-Vector3.forward);
        
        if (Input.GetKeyDown("A"))
            MoveInDirection(Vector3.left);
        if (Input.GetKeyDown("D"))
            MoveInDirection(-Vector3.left);
    }

    private void MoveInDirection(Vector3 dir)
    {
        Rb.velocity = dir * MoveSpeed ;
    }
}
