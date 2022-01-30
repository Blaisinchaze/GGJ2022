using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Turret : Defence
{
    [Header("Turret Settings")]
    public float bulletSpeed = 300;
    public GameObject bulletPrefab;

    private Vector3 offset;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        base.Update();

        switch (powerState)
        {
            case PowerState.DRAINED:
                target = null;
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

        if (target == null || Vector3.Distance(target.transform.position, transform.position) > range)
        {
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
}
