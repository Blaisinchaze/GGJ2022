using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicButton : MonoBehaviour
{
    [SerializeField] private InputManager inputs;

    private Combatant playerDevice;

    private LayerMask enemyMask;
    // Start is called before the first frame update
    void Start()
    {
        inputs.onJumpPressed += Explode;
        playerDevice = GetComponent<Combatant>();
    }

    private void Explode()
    {
        if (playerDevice.currentEnergy >= (playerDevice.maxEnergy - 0.09f))
        {
            Ray ray = new Ray();
            ray.origin = transform.position;
            var enemies = Physics.SphereCastAll(ray, 5f, 0.5f, enemyMask);
            foreach (var enemy in enemies)
            {
                enemy.transform.GetComponent<Enemy>().Die();
            }
        }
    }
}
