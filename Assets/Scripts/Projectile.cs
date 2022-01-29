using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int dmg;

    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy;
        if (other.TryGetComponent(out enemy))
        {
            enemy.GetHit(dmg);
        }

        Destroy(gameObject);
    }
}
