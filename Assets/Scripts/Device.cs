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
    public float minActiveEnergy;
    public float energyLossPerSecond;
    [Space]
    [SerializeField]
    internal float currentEnergy;
    internal Stack<PowerState> stateStack = new Stack<PowerState>();

    // For any device Start make sure you call base.Start() and your start function is an override
    internal virtual void Start()
    {
        currentEnergy = maxEnergy;
        stateStack.Push(PowerState.POWERED);
    }

    // For any device update make sure you call base.Update() and your update function is an override
    internal virtual void Update()
    {
        StateUpdate();
        SpendEnergy(energyLossPerSecond * Time.deltaTime);
    }

    // returns how much over the maxEnergy
    public float AddEnergy(float input)
    {
        float retVal = 0;
        if (currentEnergy + input > maxEnergy)
        {
            retVal = (currentEnergy + input) - maxEnergy;
        }

        currentEnergy += input;

        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);


        return retVal;
    }

    // returns how much energy is being spent
    public float SpendEnergy(float cost)
    {
        //float retVal = cost;

        if (currentEnergy - cost < 0)
        {
            return 0;
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
        if (stateStack.Count == 0)
        {
            stateStack.Push(PowerState.CHARGING);
        }
    }

    public IEnumerator StealEnergyOverTime(Device victim, float TimeFromMax)
    {
        StealEnergy(victim, victim.maxEnergy / TimeFromMax * Time.deltaTime);
        yield return 0;
    }
}
