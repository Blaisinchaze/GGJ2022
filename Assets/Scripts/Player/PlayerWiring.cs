using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerWiring : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject Head;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Device playerDevice;
    [SerializeField] private LayerMask deviceMask;
    [SerializeField] private GameObject tether;
    [SerializeField] private Tether tetherScript;
    
    [Header("Combat")] 
    [SerializeField] private Vector3 tetherPoint;

    //  Rotation stored to track where player is looking and break tether
    private Quaternion storedRot;

    public delegate void TetherAttachDel();
    public delegate void TetherDetachDel();
    public TetherAttachDel OnTetherAttach;
    public TetherDetachDel OnTetherDetach;
    
    private Vector3 tetherNormal;
    [SerializeField] private Device target;
    private bool targetAttached = false;
    public float drainTime;

    [SerializeField] AudioSource buzzNoise;
    [SerializeField] Animator weaponAnimation;

    // Start is called before the first frame update
    void Start()
    {
        inputManager.onLeftMouseClick += GetNewTarget;
        inputManager.onRightMouseClick += GetNewTarget;
        OnTetherAttach += tetherScript.OnTetherAttach;
        OnTetherDetach += tetherScript.OnTetherDetach;
        OnTetherDetach += ResetTether;
        GameManager.Instance.m_EnemyKilled.AddListener(CheckEnemyDead);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState() != GameState.PLAYING) return;

        InteractTarget();
        if (buzzNoise.isPlaying)
        {
            switch (tetherScript.currentTetherState)
            {
                case Tether.tetherAction.Neutral:
                    break;
                case Tether.tetherAction.Sending:
                    buzzNoise.pitch = Mathf.Abs((playerDevice.currentEnergy / 40) + 0.5f);
                    break;
                case Tether.tetherAction.Taking:
                    buzzNoise.pitch = Mathf.Abs((playerDevice.currentEnergy / 40) + 0.5f) * -1 ;
                    break;
                default:
                    break;
            }
        }
        }

    private void InteractTarget()
    {
        if (CatchDetach()) return;
        AttachTether();

        if (inputManager.RightMouseHeld)
        {
            StartCoroutine(playerDevice.StealEnergyOverTime(target, drainTime));
            tetherScript.currentTetherState = Tether.tetherAction.Taking;
            if (target.currentEnergy <= 1 && target.powerState == PowerState.DRAINED
                && tetherScript.currentTetherState == Tether.tetherAction.Taking) target = null;
        }
        else if (inputManager.LeftMouseHeld)
        {
            StartCoroutine(playerDevice.SendEnergyOverTime(target, drainTime));
            if (target.currentEnergy >= target.maxEnergy
                && tetherScript.currentTetherState == Tether.tetherAction.Sending) target = null;
            tetherScript.currentTetherState = Tether.tetherAction.Sending;
        }

        if (!inputManager.RightMouseHeld && tetherScript.currentTetherState == Tether.tetherAction.Taking)
        {
            StopCoroutine(playerDevice.StealEnergyOverTime(target, drainTime));
            target = null;
        }
        else if (!inputManager.LeftMouseHeld && tetherScript.currentTetherState == Tether.tetherAction.Sending)
        {
            StopCoroutine(playerDevice.SendEnergyOverTime(target, drainTime));
            target = null;
        }
    }

    private bool DetachFromDistance()
    {
        //  Measure distance between player and device, if it's too great then snap the tether
        if (Vector3.Distance(Head.transform.position, tether.transform.position) > 4f)
            return true;

        //  Measure the angle the player is looking, if it's too far then snap the tether
        var lookingAngle = Quaternion.Angle(cameraTransform.localRotation, storedRot);
        if (lookingAngle > 45)
            return true;
        return false;
    }
    /// <summary>
    /// Prevent errors from target being set to null
    /// </summary>
    private bool CatchDetach()
    {
        targetAttached = target != null;
        if (targetAttached && !DetachFromDistance()) return false;
        OnTetherDetach?.Invoke();
        return true;
    }

    private void CheckEnemyDead(Enemy enemy)
    {
        if (!targetAttached) return;
        if (target.GetComponent<Enemy>() == enemy)
            target = null;
    }
    /// <summary>
    /// Clear all the tether values once detached
    /// </summary>
    private void ResetTether()
    {
        tether.transform.parent = null;
        tether.transform.position = Vector3.zero;
        tether.transform.rotation = Head.transform.rotation;
        tether.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
        tether.SetActive(false);
        tetherPoint = Vector3.zero;
        storedRot = cameraTransform.localRotation;
        target = null;
        if (buzzNoise.isPlaying)
        {
            buzzNoise.Stop();
        }
        weaponAnimation.SetBool("Attacking", false);
    }
    /// <summary>
    /// Find a target and tether to it if not null
    /// </summary>
    private void GetNewTarget()
    {
        if (target != null) return;
        var _target = TryFindDevice();
        if (_target == null)
            return;
        target = _target;
        AttachTether();
        
    }

    /// <summary>
    /// If not attached, enable the tether. Otherwise, update tether location every update
    /// </summary>
    private void AttachTether()
    {
        if (!tether.activeSelf)
        {
            tether.SetActive(true);
            tether.transform.position = tetherPoint;
            OnTetherAttach?.Invoke();
        }

        //  Update tether location
        //tether.transform.LookAt(Head.transform.position);
        var direction = new Vector3(tether.transform.position.x - cameraTransform.position.x,
            tether.transform.position.y - cameraTransform.position.y,
            tether.transform.position.z - cameraTransform.position.z).normalized;
        storedRot = Quaternion.LookRotation(direction, Vector3.up);

        tether.transform.rotation = Quaternion.FromToRotation(transform.up, tetherNormal);
        tether.transform.parent = target.transform;
        tether.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
        tetherScript.playerLocation = firePoint.position;
        tetherScript.tetherLocation = tether.transform.position;
        if(!buzzNoise.isPlaying)
        {

            buzzNoise.Play();
        }
        weaponAnimation.SetBool("Attacking", true);
    }
    /// <summary>
    /// Launch a spherecast forward, find the closest device to the origin and assign that as the new target.
    /// </summary>
    /// <returns></returns>
    private Device TryFindDevice()
    {

        Ray ray = new Ray(firePoint.transform.position,cameraTransform.forward);
        RaycastHit hit;
        float sphereSize = 0.35f;
        float sphereDist = 20f;
        var devices = Physics.SphereCastAll(ray, sphereSize, sphereDist, 
            deviceMask, QueryTriggerInteraction.Collide);
        if (devices.Length <= 0) return null;


        float loopSphereSize = sphereSize;
        float loopShereDist = sphereDist;
        int lCatch = 30;
        int l = 0;
        Device targetDevice = null;
        while (l < lCatch )
        {
            loopSphereSize *= 0.65f;
            loopShereDist *= 0.95f;
            var nCast = Physics.SphereCastAll(ray, loopSphereSize, loopShereDist, 
                deviceMask, QueryTriggerInteraction.Collide);
            if (nCast.Length == 1)
            {
                tetherPoint = nCast[0].point;
                tetherNormal = nCast[0].normal;
                nCast[0].transform.TryGetComponent(out targetDevice);
            }

            if (nCast.Length == 0)
            {
                return targetDevice;
            }
            l++;
            devices = nCast;
        }

        //  Backup search to ensure a hit
        Dictionary<float, RaycastHit> deviceLocations = new Dictionary<float, RaycastHit>();
        float[] distances = new float[devices.Length];
        for (int i = 0; i < devices.Length; i++)
        {
            distances[i] = Vector3.Distance(firePoint.transform.position, devices[i].transform.position);
            deviceLocations.Add(distances[i], devices[i]);
        }
        System.Array.Sort(distances);
        deviceLocations.TryGetValue(distances[0], out hit);
        hit.transform.TryGetComponent(out targetDevice);
        tetherPoint = hit.point;
        tetherNormal = hit.normal;
        return targetDevice;
    }

    public Device getCurrentConnectedDevice()
    {
        return target;
    }
}
