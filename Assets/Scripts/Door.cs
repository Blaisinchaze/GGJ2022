using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Device
{
    [Header("Door Settings")]
    public float yIncrease;
    public bool isOpen;
    [Space]
    public Transform[] spawnPoints = new Transform[1];

    internal override void Update()
    {
        base.Update();
        if (powerState == PowerState.POWERED && !isOpen)
        {
            Open();
        }
    }

    public void Open()
    {
        isOpen = true;
        DoorManager.Instance.m_doorOpened.Invoke(this);
    }
}
