using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    MAINMENU,
    PRE_GAME,
    PLAYING,
    PAUSED,
    POST_GAME
}

public class GameManager : MonoBehaviour
{
    #region Singleton stuff
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }
    #endregion

    [SerializeField]
    private GameState currentState;
    private GameState prevState = GameState.MAINMENU;

    private int score = 0;

    #region Events
    public EnemyEvent m_EnemyKilled = new EnemyEvent();
    public UnityEvent m_PlayerKilled = new UnityEvent();
    public StateEvent m_StateChange = new StateEvent();
    #endregion

    private void Awake()
    {
        #region Singleton stuff
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        #endregion
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            ChangeState(GameState.MAINMENU);
        }
        else
        {
            ChangeState(GameState.PLAYING);
        }

        m_EnemyKilled.AddListener(AddScoreFromEnemyKill);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        prevState = currentState;
        currentState = newState;

        m_StateChange.Invoke(newState);
    }

    public void AddScoreFromEnemyKill(Enemy enemy)
    {
        score += enemy.scoreValue;
    }

    public GameState CurrentState()
    {
        return currentState;
    }
}

[System.Serializable]
public class StateEvent : UnityEvent<GameState>
{
}

[System.Serializable]
public class EnemyEvent : UnityEvent<Enemy>
{}
