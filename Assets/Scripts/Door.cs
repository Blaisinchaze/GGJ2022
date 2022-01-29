using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Device
{
    [Header("Door Settings")]
    public float yIncrease;
    public float speed;
    [Space]
    public bool isOpen;
    [Space]
    public Transform[] spawnPoints = new Transform[1];

    private Vector3 startPos;

    internal override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    internal override void Update()
    {
        base.Update();
        if (powerState == PowerState.POWERED && !isOpen)
        {
            Open();
        }

        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, startPos + new Vector3(0, yIncrease, 0), speed * Time.deltaTime);
        }
    }

    public void Open()
    {
        isOpen = true;
        DoorManager.Instance.m_doorOpened.Invoke(this);
    }
}
