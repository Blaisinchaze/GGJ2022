using System.Collections;
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
        //currentEnergy = maxEnergy;
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
        if (chargeProtection)
            return input;

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
        //float retVal = cost;

        if (currentEnergy - cost < 0)
            return 0;
        if (drainProtection)
            return cost;

        float retVal = cost;

        if (currentEnergy - cost < 0)
        {
            retVal = Mathf.Abs(currentEnergy - cost);
        }

        currentEnergy -= cost;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        //return retVal;
        return cost;
    }

    /// <summary>
    /// Steals energy from a victim, then returns excess energy from the thief
    /// </summary>
    /// <param name="victim">The target having energy taken</param>
    /// <param name="value">Amount of energy being taken</param>
    public void StealEnergy(Device victim, float value)
    {
        float stolenEnergy = victim.SpendEnergy(value);
        victim.AddEnergy(AddEnergy(stolenEnergy));
    }

    internal virtual void StateUpdate()
    {
        powerState = currentEnergy >= minActiveEnergy ? PowerState.POWERED : PowerState.DRAINED;
    }

    public IEnumerator StealEnergyOverTime(Device victim, float TimeFromMax)
    {
        StealEnergy(victim, victim.maxEnergy / TimeFromMax * Time.deltaTime);
        yield return 0;
    }
    
    public IEnumerator SendEnergyOverTime(Device receiver, float TimeFromMax)
    {
        receiver.StealEnergy(this, maxEnergy / TimeFromMax * Time.deltaTime);
        yield return 0;
    }
}
