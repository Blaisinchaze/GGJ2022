using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mech : Defence
{
    [Header("Mech Settings")]
    public float moveSpeed;
    public float maxDistanceFromPlayer;

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
                agent.speed = 0;
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
        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position,player.position) > maxDistanceFromPlayer)
        {
            Debug.Log("moving");
            agent.speed = moveSpeed;
        }
        else
        {
            agent.speed = 0;
        }

        if (target == null || Vector3.Distance(target.transform.position, player.position) > range)
        {
            EnemyDetection();
        }

        if (target != null)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("Mech Attack");
    }
}
