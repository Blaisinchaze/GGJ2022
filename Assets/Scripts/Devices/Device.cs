﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerState
{
    DRAINED,
    CHARGING,
    POWERED
}

public class Device : MonoBehaviour
{
    [Header("Device Settings")]
    public float maxEnergy;
    public float energyLossPerSecond;
    public float minActiveEnergy;
    [Space]
    [SerializeField]
    internal float currentEnergy;
    internal PowerState powerState;
    internal bool drainProtection;
    internal bool chargeProtection;

    // For any device Start make sure you call base.Start() and your start function is an override
    internal virtual void Start()
    {
        currentEnergy = maxEnergy;
        powerState = PowerState.POWERED;
    }

    // For any device update make sure you call base.Update() and your update function is an override
    internal virtual void Update()
    {
        StateUpdate();
        SpendEnergy(energyLossPerSecond * Time.deltaTime);
    }

    // returns how much over the maxEnergy / how much energy not used
    public float AddEnergy(float input)
    {
        if (chargeProtection) return input;

        float retVal = 0;
        if (currentEnergy + input > maxEnergy)
        {
            retVal = (currentEnergy + input) - maxEnergy;
        }

        currentEnergy += input;

        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        return retVal;
    }
    
    // returns how much energy is being not being spent
    public float SpendEnergy(float cost)
    {
        if (drainProtection) return cost;

        float retVal = cost;

        if (currentEnergy - cost < 0)
        {
            retVal = Mathf.Abs(currentEnergy - cost);
        }

        currentEnergy -= cost;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        return retVal;
    }

    internal virtual void StateUpdate()
    {
        powerState = PowerState.POWERED;
    }
}