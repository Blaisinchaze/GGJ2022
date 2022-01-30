using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject end;
    public GameObject inGame;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.m_PlayerKilled.AddListener(LoadEndScreen);
        end.SetActive(false);
    }

    // Update is called once per frame
    void LoadEndScreen()
    {
        end.SetActive(true);
        end.SetActive(true);
        text.text = GameManager.Instance.score.ToString();
    }
}
