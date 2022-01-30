using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public int healthUpgrade;
    public float speedUpgrade;
    public float drainUpgrade;
    public float batteryUpgrade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Combatant cb = other.GetComponent<Combatant>();
            cb.health += healthUpgrade;
            cb.maxHealth += healthUpgrade;
            cb.maxEnergy += batteryUpgrade;
            PlayerWiring wr = other.GetComponent<PlayerWiring>();
            wr.drainTime += drainUpgrade;
            PlayerMovement mv = other.GetComponent<PlayerMovement>();
            mv.moveSpeed += speedUpgrade;
        }
    }
}
