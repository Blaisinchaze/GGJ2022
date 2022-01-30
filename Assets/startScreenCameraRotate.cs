using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScreenCameraRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler( new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+ 0.1f, transform.rotation.z)); 
    }
}
