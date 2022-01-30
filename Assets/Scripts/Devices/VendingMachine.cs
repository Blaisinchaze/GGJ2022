using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : Device
{
    [Header("Vending Settings")]
    public GameObject prefab;
    public float cooldown;
    private float cdTimer;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        base.Update();
        if (powerState == PowerState.POWERED && cdTimer <= 0)
        {
            SpendEnergy(minActiveEnergy);
            Dispense();
            cdTimer = cooldown;
        }

        chargeProtection = cdTimer > 0;

        if (cdTimer > 0) cdTimer -= Time.deltaTime;
    }

    private void Dispense()
    {
        Instantiate(prefab, transform.position + transform.forward + transform.up, Quaternion.identity);
    }
}
