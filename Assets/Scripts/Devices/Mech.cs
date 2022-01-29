using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mech : Defence
{
    [Header("Mech Settings")]
    public float moveSpeed;
    [Space]
    public float minDistanceFromPlayer;
    public float maxAggroRange;

    NavMeshAgent agent;
    Transform player;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    internal override void Update()
    {
        base.Update();

        switch (powerState)
        {
            case PowerState.DRAINED:
                StandStill();
                break;
            case PowerState.POWERED:
                PoweredUpdate();
                break;

            default:
                break;
        }
    }

    private void PoweredUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) > minDistanceFromPlayer)
        {
            agent.SetDestination(player.position);
            agent.speed = moveSpeed;
        }
        else
        {
            StandStill();
        }

        if (target == null || Vector3.Distance(target.transform.position, transform.position) > range)
        {
            Debug.Log("enemy check");
            EnemyDetection();
        }
        else
        {
            Debug.Log("attack check");
            Attack();
        }
    }

    private void Attack()
    {
        if (attackTimer <= 0)
        {
            Debug.Log("Mech Attack");
            Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.1f);

            attackTimer = attackDelay;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void StandStill()
    {
        agent.speed = 0;
        agent.SetDestination(transform.position);
    }
}
