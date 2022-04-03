using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private static int _cntScenes;

    [SerializeField]
    private NextLevelTransition nextLevelTransition;
    
    public int CurLevel { get; private set; }

    private void Awake()
    {
        #region SINGLETON PATTERN
        if (Instance == null)
        {
            Instance = this;
            _cntScenes = SceneManager.sceneCountInBuildSettings;
        }
        else
        {
            Debug.Log($"Do not add 2 {GetType()}");
        }
        #endregion
    }
    
    private void Start()
    {
        CurLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextLevel()
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
        Debug.Log($"Load level: {curLevel}/{_cntScenes}");
        if (curLevel > _cntScenes)
        {
            return;
        }
        
        nextLevelTransition.DoTransition(NextLevelTransition.TransitionFace.Close);
        nextLevelTransition.OnTransitionComplete += () =>
        {
            DOTween.KillAll(false);
            SceneManager.LoadScene(curLevel);
        };

        GameManager.Instance.UpdateGameState(GameState.Playing);
    }
}
