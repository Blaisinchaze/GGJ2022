using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Text Boxes")]

    [SerializeField] private TextMeshProUGUI wave;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI devicePower;
    [SerializeField] private TextMeshProUGUI deviceName;

    [Header("Player Info")]
    [SerializeField] private GameObject Player;
    private Combatant playerCombatant;
    private PlayerWiring playerWiring;
    private WaveManager waveManager;
    // Start is called before the first frame update
    void Start()
    {
        playerCombatant = Player.GetComponent<Combatant>();
        playerWiring = Player.GetComponent<PlayerWiring>();
        waveManager = WaveManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        health.text = playerCombatant.health.ToString();
        wave.text = waveManager.waveNum.ToString();
        power.text = ((int)playerCombatant.currentEnergy).ToString() + "%";
        if(playerWiring.getCurrentConnectedDevice() != null)
        {

            devicePower.text = ((int)playerWiring.getCurrentConnectedDevice().currentEnergy).ToString() + "%";
            deviceName.text = playerWiring.getCurrentConnectedDevice().gameObject.name;
            
        }
        else
        {
            devicePower.text = "No Connected Device";
        }
    }
}
