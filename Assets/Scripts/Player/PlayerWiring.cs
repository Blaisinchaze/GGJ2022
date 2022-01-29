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

    // Start is called before the first frame update
    void Start()
    {
        inputManager.onLeftMouseClick += GetNewTarget;
        inputManager.onRightMouseClick += GetNewTarget;
        OnTetherAttach += tetherScript.OnTetherAttach;
        OnTetherDetach += tetherScript.OnTetherDetach;
        OnTetherDetach += ResetTether;
    }

    // Update is called once per frame
    void Update()
    {
        InteractTarget();
    }

    private void InteractTarget()
    {
        if (CatchDetach()) return;
        AttachTether();

        if (inputManager.RightMouseHeld)
            StartCoroutine(playerDevice.StealEnergyOverTime(target, 5f));
        else if (inputManager.LeftMouseHeld)
            StartCoroutine(playerDevice.SendEnergyOverTime(target, 5f));
        if (!inputManager.RightMouseHeld)
            StopCoroutine(playerDevice.StealEnergyOverTime(target, 5f));
        else if (!inputManager.LeftMouseHeld)  
            StopCoroutine(playerDevice.SendEnergyOverTime(target, 5f));
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
    }
    private void GetNewTarget()
    {
        if (target != null) return;
        var _target = TryFindDevice();
        if (_target == null)
            return;
        target = _target;
        AttachTether();
        
    }

    private void AttachTether()
    {
        if (!tether.activeSelf)
        {
            tether.SetActive(true);
            tether.transform.position = tetherPoint;
            OnTetherAttach?.Invoke();
        }

        //tether.transform.LookAt(Head.transform.position);
        var direction = new Vector3(tether.transform.position.x - cameraTransform.position.x,
            tether.transform.position.y - cameraTransform.position.y,
            tether.transform.position.z - cameraTransform.position.z).normalized;
        storedRot = Quaternion.LookRotation(direction, Vector3.up);

        tether.transform.rotation = Quaternion.FromToRotation(transform.up, tetherNormal);
        tether.transform.parent = target.transform;
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
}
