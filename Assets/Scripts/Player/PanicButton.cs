using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicButton : MonoBehaviour
{
    [SerializeField] private InputManager inputs;

    private Combatant playerDevice;

    [SerializeField] private Transform camera;
    [SerializeField] private LayerMask enemyMask;

    [Header("Explosion")]
    public float explosionRange;
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
            ray.origin = camera.position;
            ray.direction = camera.forward;
            var enemies = Physics.SphereCastAll(ray, explosionRange, 1f, enemyMask);
            foreach (var enemy in enemies)
            {
                if (!enemy.transform.CompareTag("Enemy")) continue;
                enemy.transform.TryGetComponent<Enemy>(out Enemy comp);
                comp.GetHit(99);
            }

            playerDevice.SpendEnergy(playerDevice.currentEnergy);
        }
    }
}
