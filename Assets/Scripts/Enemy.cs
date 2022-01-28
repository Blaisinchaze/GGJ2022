using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Device
{
    [Header("Enemy Settings")]
    public float chargeRatePerSecond;
    public float minDistanceFromPlayer;

    NavMeshAgent agent;
    Transform player;
    bool shielded;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        shielded = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        StateUpdate();

        switch (stateStack.Peek())
        {
            case PowerState.DRAINED:
                break;
            case PowerState.CHARGING:
                currentEnergy += chargeRatePerSecond * Time.deltaTime;
                break;
            case PowerState.POWERED:
                UpdatePathing(player.position);
                break;
            default:
                break;
        }
    }

    override internal void StateUpdate()
    {
        switch (stateStack.Peek())
        {
            case PowerState.DRAINED:
                stateStack.Pop();
                stateStack.Push(PowerState.CHARGING);
                break;

            case PowerState.CHARGING:
                if (currentEnergy == maxEnergy)
                {
                    stateStack.Clear();
                    stateStack.Push(PowerState.POWERED);
                }
                break;

            case PowerState.POWERED:
                if (currentEnergy == 0)
                {
                    stateStack.Push(PowerState.CHARGING);
                }
                break;

            default:
                break;
        }
        
    }

    private void UpdatePathing(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
