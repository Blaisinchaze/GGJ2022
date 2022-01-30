using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int dmg;
    public GameObject parent;

    public void Start()
    {
        Destroy(gameObject, 15);
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy enemy;
        if (other.TryGetComponent(out enemy))
        {
            enemy.GetHit(dmg);
        }

        if (other.gameObject != parent)
        {
            Destroy(gameObject);
        }
    }
}
