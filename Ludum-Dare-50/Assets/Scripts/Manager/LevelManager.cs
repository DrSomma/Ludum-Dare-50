using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int CurLevel { get; private set; }

    private void Awake()
    {
        #region SINGLETON PATTERN
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError($"Do not add 2 {this.GetType()}");
        }
        #endregion
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Endpoint.OnLevelComplete += LoadNextLevel;

        CurLevel = 0;
    }

    private void LoadNextLevel()
    {
        CurLevel++;
        LoadLevel(CurLevel);
    }

    public void ReloadCurrentScene()
    {
        LoadLevel(curLevel: CurLevel);
    }

    private void LoadLevel(int curLevel)
    {
        //Todo: level transition
        
        var cntScenes = SceneManager.sceneCount;
        if (curLevel > cntScenes)
        {
            return;
        }

        SceneManager.LoadScene(curLevel);
    }
}
