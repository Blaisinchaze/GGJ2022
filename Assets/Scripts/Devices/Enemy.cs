using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Device
{
    [Header("Enemy Settings")]
    public float chargeRatePerSecond;
    public float overchargeDuration;
    [Space]
    public float attackDistance;
    public float attackDelay;
    [Space]
    public float moveSpeed;
    public float health;
    [Space]
    public int scoreValue;

    private float overchargeTimer = 0;
    private float attackTimer;

    NavMeshAgent agent;
    Transform player;

    override internal void Start()
    {
        base.Start();
        overchargeTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    override internal void Update()
    {
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);

        if (GameManager.Instance.CurrentState() != GameState.PLAYING) return;
        base.Update();
        
        switch (powerState)
        {
            case PowerState.CHARGING:
                currentEnergy += chargeRatePerSecond * Time.deltaTime;
                agent.speed = 0;
                break;

            case PowerState.POWERED:
                PoweredUpdate();
                break;

            default:
                break;
        }

        drainProtection = powerState != PowerState.POWERED || overchargeTimer > 0;
        attackTimer -= Time.deltaTime;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.m_EnemyKilled.Invoke(this);
        Destroy(gameObject);
    }

    private void TakeDamage(float dmg)
    {
        if (overchargeTimer >= 0) return;

        health -= dmg;

        if (health <= 0)
        {
            Die();
        }
    }

    private void PoweredUpdate()
    {
        agent.SetDestination(player.position);

        if (Vector3.Distance(player.position, transform.position) <= attackDistance)
        {
            agent.speed = 0;
            if (attackTimer <= 0)
            {
                //attack
                attackTimer = attackDelay;
            }
        }
        else
        {
            agent.speed = moveSpeed;
            //any additional move stuff goes here
        }


        if (overchargeTimer > 0)
        {
            overchargeTimer -= Time.deltaTime;
            currentEnergy = maxEnergy;
            // overcharge actions
        }
    }

    override internal void StateUpdate()
    {
        switch (powerState)
        {
            case PowerState.DRAINED:
                powerState = PowerState.CHARGING;
                break;

            case PowerState.CHARGING:
                if (currentEnergy >= maxEnergy)
                {
                    powerState = PowerState.POWERED;
                    overchargeTimer = overchargeDuration;
                }
                break;

            case PowerState.POWERED:
                if (currentEnergy <= 0)
                {
                    powerState = PowerState.CHARGING;
                }
                break;

            default:
                break;
        }

        //Debug.Log("State = " + powerState);
        
    }
}
