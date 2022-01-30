using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence : Device
{
    [Header("Defence Settings")]
    public int strength;
    public float attackDelay;
    public float hitStunDuration;
    public float range;
    [Space]
    public Combatant target;

    internal float attackTimer;
    private float width = 0.1f;

    internal override void Start()
    {
        width = GetComponent<Collider>().bounds.size.magnitude;
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
                if (currentEnergy <= 0.25f)
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

        Combatant closest = target;

        foreach (Collider hit in hits)
        {
            if (hit.tag == "Enemy")
            {
                if (closest == null || Vector3.Distance(transform.position, hit.transform.position) <
                    Vector3.Distance(transform.position, closest.transform.position))
                {
                    Ray ray = new Ray(transform.position, hit.transform.position - transform.position);
                    RaycastHit rh;
                    if (!Physics.Raycast(ray, out rh, Vector3.Distance(transform.position, hit.transform.position), ~deviceMask))
                    {
                        Debug.DrawLine(transform.position, hit.transform.position - transform.position, Color.blue, 0.1f);
                        closest = hit.gameObject.GetComponent<Combatant>();
                    }
                }
            }
        }

        target = closest;
    }
}
