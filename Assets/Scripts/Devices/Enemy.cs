using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Combatant
{
    [Header("Enemy Settings")]
    public float chargeRatePerSecond;
    public float overchargeDuration;
    [Space]
    public float attackDistance;
    public float attackDelay;
    [Space]
    public float moveSpeed;
    [Space]
    public int scoreValue;
    [Space]
    public Material deadMat;

    internal bool agentActive = false;

    private Renderer rend;

    private float overchargeTimer = 0;
    private float attackTimer;

    NavMeshAgent agent;
    Combatant player;
    Transform playerT;
    EnemyFaceUpdate faceHandler;

    override internal void Start()
    {
        base.Start();
        overchargeTimer = 0;
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerT.GetComponentInParent<Combatant>();
        agent = GetComponent<NavMeshAgent>();
        faceHandler = GetComponentInChildren<EnemyFaceUpdate>();
        agent.updateRotation = false;
        agent.enabled = agentActive;
        rend = GetComponent<Renderer>();
    }

    override internal void Update()
    {
        agent.enabled = agentActive;
        transform.rotation = Quaternion.LookRotation(playerT.position - transform.position);
        isAlive = health > 0;

        if (GameManager.Instance.CurrentState() != GameState.PLAYING || !isAlive) return;
        base.Update();
        
        switch (powerState)
        {
            case PowerState.CHARGING:
                currentEnergy += chargeRatePerSecond * Time.deltaTime;
                StandStill();
                faceHandler.QueueFace(Face.DRAINED);
                break;

            case PowerState.POWERED:
                PoweredUpdate();
                break;

            default:
                break;
        }

        drainProtection = powerState != PowerState.POWERED || overchargeTimer > 0;
        attackTimer -= Time.deltaTime;

    }

    public override void Die()
    {
        isAlive = false;
        GameManager.Instance.m_EnemyKilled.Invoke(this);
        rend.material = deadMat;
        faceHandler.gameObject.SetActive(false);
        gameObject.layer = 1 << 0;
        GetComponent<Collider>().enabled = false;
        //Destroy(this);
        //Destroy(gameObject, 1);
    }

    public override void GetHit(int dmg)
    {
        if (overchargeTimer > 0 || invulnerable) return;

        health -= dmg;

        if (health <= 0)
        {
            Die();
        }
    }

    private void PoweredUpdate()
    {
        agent.SetDestination(playerT.position);

        if (Vector3.Distance(playerT.position, transform.position) <= attackDistance)
        {
            StandStill();
            agent.speed = 0;
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = attackDelay;
                faceHandler.QueueFace(Face.ATTACKING);
                faceHandler.QueueFace(Face.IN_RANGE);
            }
        }
        else
        {
            agent.speed = moveSpeed;
            faceHandler.QueueFace(Face.NEUTRAL);
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
                if (currentEnergy <= 0.1f)
                {
                    powerState = PowerState.CHARGING;
                }
                break;

            default:
                break;
        }

        //Debug.Log("State = " + powerState);
        
    }

    public void Attack()
    {
        player.GetHit(outDmg);
    }

    private void StandStill()
    {
        agent.speed = 0;
        agent.SetDestination(transform.position);
    }
}
