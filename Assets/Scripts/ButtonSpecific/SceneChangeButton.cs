using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour
{
    public string sceneName;
    public GameState state;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.ChangeScene(sceneName));
        GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.ChangeState(state));
    }
}
