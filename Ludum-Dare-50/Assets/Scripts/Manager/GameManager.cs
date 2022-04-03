using System;
using System.Collections;
using System.Collections.Generic;
using Traps;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Collectable.OnCollect += OnCollect;
    }

    public void UpdateGameState(GameState newState)
    {
        if(CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.Playing:
                break;
            case GameState.OnEndpoint:
                break;
            case GameState.LevelComplete:
                LevelManager.Instance.LoadNextLevel();
                break;
            case GameState.Dead:
                break;
            case GameState.Reload:
                Reload();
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        OnGameStateChange?.Invoke(CurrentState);
    }

    private void OnCollect()
    {
        Debug.Log("you got one");
    }

    private void Reload()
    {
        Debug.Log("Reload");
        LevelManager.Instance.ReloadCurrentScene();
    }
}

public enum GameState
{
    Playing,
    LevelComplete,
    Dead,
    Reload,
    OnEndpoint
}