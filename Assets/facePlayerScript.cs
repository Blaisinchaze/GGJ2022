using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facePlayerScript : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
    }
}
