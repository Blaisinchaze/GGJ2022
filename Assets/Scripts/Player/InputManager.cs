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
    public Vector2 MovementDirection { get; private set; }= Vector2.zero;
    

    /// <summary>
    /// Look direction determined from moving the mouse
    /// </summary>
    public Vector2 LookDirection { get; private set; } = Vector2.zero;

    /// <summary>
    /// Delegate to immediately execute jump when space is pressed
    /// </summary>
    public delegate void JumpPressed();
    public JumpPressed onJumpPressed;

    public bool sprintPressed { get; private set; }= false;

    
    public delegate void MouseClicked();
    public MouseClicked onLeftMouseClick;
    public MouseClicked onRightMouseClick;
    
    public bool LeftMouseHeld { get; private set; }
    public bool RightMouseHeld { get; private set; }

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
        CheckForLeftMouseClick();
        CheckForRightMouseClick();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
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
            onJumpPressed?.Invoke();
    }

    private void CheckForLeftMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
            onLeftMouseClick?.Invoke();
        if (Input.GetMouseButton(0))
            LeftMouseHeld = true;
        if (Input.GetMouseButtonUp(0))
            LeftMouseHeld = false;
    }
    private void CheckForRightMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
            onRightMouseClick?.Invoke();
        if (Input.GetMouseButton(1))
            RightMouseHeld = true;
        if (Input.GetMouseButtonUp(1))
            RightMouseHeld = false;
    }
    

    private void CheckForSprint()
    {
        sprintPressed = Input.GetKey(KeyCode.LeftShift);
    }

    #endregion
}
