using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    
    [Header("Combat")] 
    [SerializeField] private Vector3 tetherPoint;

    private Vector3 tetherNormal;
    [SerializeField] private Device target;
    private bool targetAttached = false;
    public float drainTime;

    // Start is called before the first frame update
    void Start()
    {
        inputManager.onLeftMouseClick += GetNewTarget;
        inputManager.onRightMouseClick += GetNewTarget;
    }

    // Update is called once per frame
    void Update()
    {
        InteractTarget();
    }

    private void InteractTarget()
    {
        if (CatchDetach()) return;
        //if (!targetAttached) return;
        AttachTether();

        if (inputManager.LeftMouseHeld)
        {
            StartCoroutine(playerDevice.StealEnergyOverTime(target, 5f));
            print("Player: " +playerDevice.currentEnergy +", Enemy: " +target.currentEnergy);
        }
        else if (inputManager.RightMouseHeld)
        {
            target.StealEnergy(playerDevice, 1);
            print("Player: " +playerDevice.currentEnergy +", Enemy: " +target.currentEnergy);
        } 
        if (!inputManager.LeftMouseHeld)
        {
            StopCoroutine(playerDevice.StealEnergyOverTime(target, 5f));
        }
        else if (!inputManager.RightMouseHeld)  
        {
        }
    }

    private bool DetachFromDistance()
    {
        if (Vector3.Distance(Head.transform.position, target.transform.position) > 1.5f)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// Prevent errors from target being set to null
    /// </summary>
    private bool CatchDetach()
    {
        targetAttached = target != null;
        if (targetAttached && !DetachFromDistance()) return false;
        tether.SetActive(false);
        tether.transform.parent = null;
        tether.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
        tetherPoint = Vector3.zero;
        target = null;
        return true;
    }
    private void GetNewTarget()
    {
        if (target != null) return;
        var _target = TryFindDevice();
        if (_target == null)
            return;
        target = _target;
    }

    private void AttachTether()
    {
        tether.SetActive(true);
        tether.transform.position = tetherPoint;
        //tether.transform.LookAt(Head.transform.position);
        tether.transform.rotation = Quaternion.FromToRotation(transform.up, tetherNormal);
        tether.transform.parent = target.transform;
    }
    /// <summary>
    /// Launch a spherecast forward, find the closest device to the origin and assign that as the new target.
    /// </summary>
    /// <returns></returns>
    private Device TryFindDevice()
    {
        if (!Physics.CheckSphere(firePoint.position, 0.5f, deviceMask))
            return null;
        
        Ray ray = new Ray(firePoint.transform.position,cameraTransform.forward);
        RaycastHit hit;
        float sphereSize = 0.35f;
        float sphereDist = 1.25f;
        var devices = Physics.SphereCastAll(ray, sphereSize, sphereDist, 
            deviceMask, QueryTriggerInteraction.Collide);
        Debug.DrawRay(ray.origin, ray.direction * 1.25f);


        float loopSphereSize = sphereSize;
        float loopShereDist = sphereDist;
        int lCatch = 30;
        int l = 0;
        Device newTarget = new Device();
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
                newTarget = nCast[0].transform.GetComponent<Device>();
            }

            if (nCast.Length == 0)
            {
                return newTarget;
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
        Device targetDevice = null;
        hit.transform.TryGetComponent(out targetDevice);
        tetherPoint = hit.point;
        tetherNormal = hit.normal;
        return targetDevice;
    }
}
