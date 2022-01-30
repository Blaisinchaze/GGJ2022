using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Tether : MonoBehaviour
{
    //  Particle systems for sending and taking energy.
    [HideInInspector] public Vector3 playerLocation;
    [HideInInspector] public Vector3 tetherLocation;
    [SerializeField] private List<GameObject> TrailObjects;
    private List<TrailMover> trailScripts = new List<TrailMover>();
    [SerializeField] [ColorUsage(true, true)]private Color hdrColorSend;
    [SerializeField] [ColorUsage(true, true)] private Color hdrColorTake;

    public enum tetherAction
    {
        Neutral = 0,
        Sending,
        Taking
    }

    public tetherAction currentTetherState;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject jointB;
    [SerializeField] private GameObject jointA;
    [SerializeField] private GameObject tether;
    [SerializeField] private GameObject playerHands;
    
    // Start is called before the first frame update
    void Start()
    {
        lineObject.transform.parent = null;
        jointA.SetActive(false);
        jointB.SetActive(false);
        lineObject.SetActive(false);
        foreach (var item in TrailObjects)
        {
            trailScripts.Add(item.GetComponent<TrailMover>());
        }

    }

    public void OnTetherAttach()
    {
        jointB.SetActive(true);
        jointA.SetActive(true);
        //lineObject.SetActive(true);
        lineObject.transform.position = Vector3.zero;

    }

    public void OnTetherDetach()
    {
        lineObject.SetActive(false);
        jointA.SetActive(false);
        jointB.SetActive(false);
        currentTetherState = tetherAction.Neutral;
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentTetherState)
        {
                case tetherAction.Neutral:
                    for (int i = 0; i < TrailObjects.Count; i++)
                    {
                        var trailObject = TrailObjects[i];
                        var trailScript = trailScripts[i];
                        trailScript.LocationA = playerLocation;
                        trailScript.LocationB = playerLocation;
                        trailScript.StopCoroutine(trailScript.MoveToLocation());
                        trailObject.SetActive(false);
                    }

                    break;
                case tetherAction.Sending:
                    ActivateTether(playerHands, tether);
                    StartCoroutine(StartTrailMoving());
                    break;
                case tetherAction.Taking:
                    ActivateTether(tether, playerHands);
                    StartCoroutine(StartTrailMoving());
                    break;
            }
    }

    private void ActivateTether(GameObject sender, GameObject receiver)
    {
        jointB.transform.position = receiver.transform.position;
        jointA.transform.position = sender.transform.position;
        lineObject.GetComponent<LineRenderer>().SetPositions(new[] {jointA.transform.position, jointB.transform.position});

        foreach (var sc in trailScripts)
        {
            sc.LocationA = jointA.transform.position;
            sc.LocationB = jointB.transform.position;
        }
    }
    private IEnumerator StartTrailMoving()
    {
        for (int i = 0; i < TrailObjects.Count; i++)
        {
            var trailObject = TrailObjects[i];
            var trailScript = trailScripts[i];
            if (!trailObject.activeSelf)
            {
                trailObject.SetActive(true);
                trailScript.LocationA = jointA.transform.position;
                trailScript.LocationB = jointB.transform.position;
                var setting = trailScript.VFX.GetComponent<Renderer>();
                if (currentTetherState == tetherAction.Sending)
                    
                    setting.material.SetColor("_EmissionColor", hdrColorSend * 2f); 
                else if (currentTetherState == tetherAction.Taking)
                    setting.material.SetColor("_EmissionColor", hdrColorTake * 2f); 

                    StartCoroutine(trailScript.MoveToLocation());
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
