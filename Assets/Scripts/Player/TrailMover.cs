using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMover : MonoBehaviour
{
    [HideInInspector] public Vector3 LocationA, LocationB;
    [SerializeField] private float lerpSpeed;
    private float lerpVal;
    /// <summary>
    /// The required distance between this object and the destination before it loops back to the start
    /// </summary>
    [SerializeField] private float distanceToReset;

    public ParticleSystem VFX;

    private void OnDisable()
    {
        lerpVal = 0;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public IEnumerator MoveToLocation()
    {
        transform.position = LocationA;
        while (transform.position != LocationB)
        {
            if (VFX.isStopped) VFX.Play();
            if (Vector3.Distance(transform.position, LocationB) < distanceToReset)
            {
                lerpVal = 0;
                VFX.Stop(true);
            }

            transform.position = Vector3.Lerp(LocationA, LocationB, lerpVal);
            lerpVal += lerpSpeed * Time.deltaTime;
            yield return 0;
        }

    }
}
