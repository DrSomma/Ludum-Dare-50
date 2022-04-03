using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
            Debug.Log($"Do not add 2 {this.GetType()}");
        }
        #endregion
    }

    private void Start()
    {
        Endpoint.OnLevelComplete += LoadNextLevel;
        CurLevel = SceneManager.GetActiveScene().buildIndex;
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

        DOTween.KillAll(false);
        
        var cntScenes = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"Load level: {curLevel}/{cntScenes}");
        if (curLevel > cntScenes)
        {
            return;
        }

        SceneManager.LoadScene(curLevel);
    }
}
