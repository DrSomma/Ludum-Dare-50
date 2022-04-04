using System;
using UnityEngine;

namespace Manager
{
    public class UiControllsManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup controlls;

        private void Start()
        {
            GameManager.Instance.OnGameStateChange += OnStart;
        }

        private void OnStart(GameState state)
        {
            if (state == GameState.Playing)
            {
                Debug.Log("playinggg setaktic");
                controlls.interactable = true;
            }
            else
            {
                controlls.interactable = false;
            }
        }

        public void OnHomeClicked()
        {
            controlls.interactable = false;
            LevelManager.Instance.LoadMainMenu();
        }
        
        public void OnAudioClicked(bool status)
        {
            if (status)
            {
                SoundManager.Instance.UnmuteAllSounds();
            }
            else
            {
                SoundManager.Instance.MuteAllSounds();
            }
        }
        
        public void OnSkipClicked()
        {
            controlls.interactable = false;
            LevelManager.Instance.LoadNextLevel();
        }
    }
}
