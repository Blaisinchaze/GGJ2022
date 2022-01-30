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
    [Space]
    public float bulletSpeed = 100;
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    Transform player;
    private Vector3 offset;

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

        if (target == null || Vector3.Distance(target.transform.position, transform.position) > range || !target.isAlive)
        {
            target = null;
            EnemyDetection();
        }
        else
        {
            Attack();
        }

        if (target != null)
        {
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        }
    }

    private void Attack()
    {
        if (attackTimer <= 0)
        {
            //target.GetComponent<Combatant>().GetHit(strength);
            attackTimer = attackDelay;
            offset = (target.transform.position - transform.position).normalized * 0.5f;
            GameObject projectile = Instantiate(bulletPrefab, transform.position + offset, Quaternion.identity);
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.dmg = strength;
            proj.parent = gameObject;
            projectile.GetComponent<Rigidbody>().AddForce(offset * bulletSpeed);
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
