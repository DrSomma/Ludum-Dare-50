using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
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
            else if(Instance != this)
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

            GameManager.Instance.UpdateGameState(GameState.Loading);
            StartCoroutine(StartLevelLoad(curLevel));
        }

        IEnumerator StartLevelLoad(int curLevel)
        {
            nextLevelTransition.DoTransition(NextLevelTransition.TransitionFace.Close);
            yield return new WaitWhile(() => nextLevelTransition.IsRunning);
            OnTransitionComplete(curLevel);
            yield return null;
        }

        private void OnTransitionComplete(int curLevel)
        {
            DOTween.KillAll(false);
            SceneManager.LoadScene(curLevel);
            nextLevelTransition.DoTransition(NextLevelTransition.TransitionFace.Open,
                () =>
                {
                    GameManager.Instance.UpdateGameState(GameState.Playing);
                });
        }

        public void LoadMainMenu()
        {
            LoadLevel(0);
        }
    }
}
