using System;
using System.Collections;
using System.Collections.Generic;
using Traps;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region SINGLETON PATTERN

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject(name: "GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    #endregion

    public GameState CurrentState { get; private set; }
    public static event Action<GameState> OnGameStateChange;
    
    
    private void Start()
    {
        Collectable.OnCollect += OnCollect;
    }

    public void UpdateGameState(GameState newState)
    {
        if(CurrentState == newState)
            return;
        
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.Playing:
                break;
            case GameState.LevelComplete:
                LevelManager.Instance.LoadNextLevel();
                break;
            case GameState.Dead:
                KillPlayer();
                break;
            
            default: throw new ArgumentOutOfRangeException();
        }
        
        OnGameStateChange?.Invoke(CurrentState);
    }

    private void OnCollect()
    {
        Debug.Log("you got one");
    }

    private void KillPlayer()
    {
        Debug.Log("KillPlayer");
        LevelManager.Instance.ReloadCurrentScene();
    }
}

public enum GameState
{
    Playing,
    LevelComplete,
    Dead
}