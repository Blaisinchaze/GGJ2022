using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InputManager : MonoBehaviour
{
    //  ====    Inputs
    
    /// <summary>
    /// Movement direction determined from pressing WASD
    /// </summary>
    /// <returns></returns>
    private Vector2 MovementDirection = Vector2.zero;

    /// <summary>
    /// Look direction determined from moving the mouse
    /// </summary>
    private Vector2 LookDirection = Vector2.zero;

    /// <summary>
    /// Delegate to immediately execute jump when space is pressed
    /// </summary>
    public delegate void JumpPressed();
    public JumpPressed onJumpPressed;

    private bool sprintPressed = false;
    
    //  ====    End of Inputs
    
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
        CheckForJump();
        CheckForSprint();
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

    public bool GetSprinting()
    {
        return sprintPressed;
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

    private void CheckForJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onJumpPressed?.Invoke();
        }
    }

    private void CheckForSprint()
    {
        sprintPressed = Input.GetKey(KeyCode.LeftShift);
    }

    #endregion
}
