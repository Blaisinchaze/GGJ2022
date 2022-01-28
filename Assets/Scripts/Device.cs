using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerState
{
    DRAINED,
    CHARGING,
    POWERED
}

public abstract class Device : MonoBehaviour
{
    [Header("Device Settings")]
    public float maxEnergy;
    public float minActiveEnergy;
    public float energyLossPerSecond;
    [Space]
    internal float currentEnergy;
    internal Stack<PowerState> stateStack;

    // Start is called before the first frame update
    void Start()
    {
        currentEnergy = maxEnergy;
        stateStack.Push(PowerState.POWERED);
    }

    // Update is called once per frame
    void Update()
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
        float retVal = cost;

        if (currentEnergy - cost < 0)
        {
            retVal = currentEnergy;
        }

        currentEnergy -= cost;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        return retVal;
    }

    abstract internal void StateUpdate();
}
