using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence : Device
{
    [Header("Defence Settings")]
    public float strength;
    public float attackDelay;
    public float hitStunDuration;
    public float range;
    [Space]
    public GameObject target;

    private float attackTimer;

    internal override void Start()
    {
        currentEnergy = 0;
        powerState = PowerState.DRAINED;
    }

    internal override void Update()
    {
        base.Update();
        if (powerState == PowerState.DRAINED) return;
    }

    internal override void StateUpdate()
    {
        switch (powerState)
        {
            case PowerState.DRAINED:
                if (currentEnergy >= minActiveEnergy)
                {
                    powerState = PowerState.POWERED;
                }
                break;
            case PowerState.POWERED:
                if (currentEnergy <= 0)
                {
                    powerState = PowerState.DRAINED;
                }
                break;
            default:
                break;
        }
    }

    internal void EnemyDetection()
    {
        LayerMask deviceMask = 1 << LayerMask.NameToLayer("Device");
        List<Collider> hits = new List<Collider>(Physics.OverlapSphere(transform.position, range, deviceMask));

        GameObject closest = null;

        foreach (Collider hit in hits)
        {
            if (hit.tag == "Enemy")
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <
                    Vector3.Distance(transform.position, closest.transform.position) || closest == null)
                {
                    Ray ray = new Ray(transform.position, hit.transform.position - transform.position);
                    RaycastHit rh;
                    if (Physics.Raycast(ray, out rh, range, ~deviceMask))
                    {
                        closest = hit.gameObject;
                    }
                }
            }
        }

        if (closest != null) target = closest;
    }
}
