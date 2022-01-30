using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    //  Particle systems for sending and taking energy.
    [SerializeField] private ParticleSystem SendFX;

    [SerializeField] private ParticleSystem TakeFX;


    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject jointB;
    [SerializeField] private GameObject jointA;
    [SerializeField] private GameObject tether;
    [SerializeField] private GameObject playerHands;
    [SerializeField] private GameObject playerGun;
    //[SerializeField] private animator gunAnimator;
    private bool Activated;
    
    // Start is called before the first frame update
    void Start()
    {
        lineObject.transform.parent = null;
        jointA.SetActive(false);
        jointB.SetActive(false);
        lineObject.SetActive(false);
        //gunAnimator = playerGun.GetComponent<Animator>();
    }

    public void OnTetherAttach()
    {
        jointB.SetActive(true);
        jointA.SetActive(true);
        lineObject.SetActive(true);
        Activated = true;
    }

    public void OnTetherDetach()
    {
        playerGun.SetActive(true);
        //playerGun.setBool("Attacking", false);
        lineObject.SetActive(false);
        jointA.SetActive(false);
        jointB.SetActive(false);
        Activated = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Activated)
        {
            
            lineObject.transform.position = Vector3.zero;
            jointB.transform.position = tether.transform.position;
            jointA.transform.position = playerHands.transform.position;
            lineObject.GetComponent<LineRenderer>().SetPositions(new[] {jointA.transform.position, jointB.transform.position});
            
            playerGun.SetActive(false);   
            //playerGun.setBool("Attacking", true);
            lineObject.SetActive(true);
            jointA.SetActive(true);
            jointB.SetActive(true);

        }
        
    }
}
