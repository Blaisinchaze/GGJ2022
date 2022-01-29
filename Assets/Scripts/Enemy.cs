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
    public float overchargeDuration;

    NavMeshAgent agent;
    Transform player;
    float overchargeTimer = 0;
    Rigidbody rb;

    // Start is called before the first frame update
    override internal void Start()
    {
        base.Start();
        overchargeTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    // Update is called once per frame
    override internal void Update()
    {
        base.Update();
        switch (stateStack.Peek())
        {
            case PowerState.CHARGING:
                currentEnergy += chargeRatePerSecond * Time.deltaTime;
                agent.speed = 0;
                break;

            case PowerState.POWERED:
                UpdatePathing(player.position);
                agent.speed = moveSpeed;
                break;

            default:
                break;
        }

        if (overchargeTimer > 0)
        {
            overchargeTimer -= Time.deltaTime;
            // overcharge settings
        }

        transform.rotation = Quaternion.LookRotation(player.position - transform.position);
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
                if (currentEnergy >= maxEnergy)
                {
                    stateStack.Clear();
                    stateStack.Push(PowerState.POWERED);
                    overchargeTimer = overchargeDuration;
                }
                break;

            case PowerState.POWERED:
                if (currentEnergy <= 0)
                {
                    stateStack.Push(PowerState.CHARGING);
                }
                break;

            default:
                break;
        }

        Debug.Log("State = " + stateStack.Peek());
        
    }

    private void UpdatePathing(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
