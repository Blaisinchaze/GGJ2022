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
    public float moveSpeed;

    NavMeshAgent agent;
    Transform player;
    bool shielded;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        shielded = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (stateStack.Peek())
        {
            case PowerState.DRAINED:
            case PowerState.CHARGING:
                currentEnergy += chargeRatePerSecond * Time.deltaTime;
                agent.speed = 0;
                break;
            case PowerState.POWERED:
                Debug.Log("Beep");
                UpdatePathing(player.position);
                agent.speed = moveSpeed;
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
