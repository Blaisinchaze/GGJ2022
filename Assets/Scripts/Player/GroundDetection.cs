using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] private bool isGrounded = false;

    [SerializeField] private LayerMask groundMask;
    
    private void FixedUpdate()
    {
        isGrounded = (Physics.CheckSphere(transform.position, 0.5f, groundMask));
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

}
