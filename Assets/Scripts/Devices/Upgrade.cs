using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public int healthUpgrade;
    public float speedUpgrade;
    public float drainUpgrade;
    public float batteryUpgrade;

    private void Update()
    {

        transform.Rotate(Vector3.up, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Combatant cb = other.GetComponentInParent<Combatant>();
            cb.health += healthUpgrade;
            cb.maxHealth += healthUpgrade;
            cb.maxEnergy += batteryUpgrade;
            PlayerWiring wr = other.GetComponentInParent<PlayerWiring>();
            wr.drainTime += drainUpgrade;
            PlayerMovement mv = other.GetComponentInParent<PlayerMovement>();
            mv.moveSpeed += speedUpgrade;

            Destroy(gameObject);
        }
    }
}
