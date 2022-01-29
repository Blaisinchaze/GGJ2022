using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Movement direction determined from pressing WASD
    /// </summary>
    /// <returns></returns>
    private Vector2 MovementDirection = Vector2.zero;

    /// <summary>
    /// Look direction determined from moving the mouse
    /// </summary>
    private Vector2 LookDirection = Vector2.zero;

    private bool jumpPressed = false;
    
    //Replace this when we have a gamestate manager
    public bool InGame = true;
    
    void Start()
    {
        if (InGame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void Update()
    {

        UpdateLookDirection();
        UpdateMovementDirection();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    #region Public Calls

    public Vector2 GetWASD()
    {
        return MovementDirection;
    }

    public Vector2 GetMouseDir()
    {
        return LookDirection;
    }

    public bool GetIsJumping()
    {
        return jumpPressed;
    }
    #endregion
    #region Local Stuff
    private void UpdateMovementDirection()
    {
        MovementDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            MovementDirection = new Vector2(MovementDirection.x, MovementDirection.y + 1);
        if (Input.GetKey(KeyCode.S))
            MovementDirection = new Vector2(MovementDirection.x, MovementDirection.y - 1);
        
        if (Input.GetKey(KeyCode.A))
            MovementDirection = new Vector2(MovementDirection.x - 1, MovementDirection.y);
        if (Input.GetKey(KeyCode.D))
            MovementDirection = new Vector2(MovementDirection.x + 1, MovementDirection.y);
    }

    private void UpdateLookDirection()
    {
        LookDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void UpdateJump()
    {
        jumpPressed = false;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }
    }


    #endregion
}
