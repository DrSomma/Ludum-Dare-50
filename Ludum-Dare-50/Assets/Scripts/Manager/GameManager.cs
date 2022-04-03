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

    private void Start()
    {
        Trap.OnTrapHit += KillPlayer;
        Collectable.OnCollect += OnCollect;
    }

    private void OnCollect()
    {
        Debug.Log("you got one");
    }

    private void KillPlayer()
    {
        LevelManager.Instance.ReloadCurrentScene();
    }
}
