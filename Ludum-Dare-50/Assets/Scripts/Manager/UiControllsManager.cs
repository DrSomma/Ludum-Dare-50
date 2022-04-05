using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class UiControllsManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup controlls;

        public Sprite MuteAudioSprite;
        public Sprite UnmuteAudioSprite;

        private Image _audioButtonImage;
        private bool _isSoundMuted;

        private void Start()
        {
            GameManager.Instance.OnGameStateChange += OnStart;
            _audioButtonImage = controlls.transform.Find("audio")?.gameObject.GetComponent<Image>();
            if (_audioButtonImage == null)
            {
                Debug.LogError("Image of AudioButton was not found!");
            }
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

        public void OnAudioClicked()
        {
            if (_isSoundMuted)
            {
                SoundManager.Instance.UnmuteAllSounds();
                _isSoundMuted = false;
                if (_audioButtonImage != null)
                {
                    _audioButtonImage.sprite = MuteAudioSprite;
                }
            }
            else
            {
                SoundManager.Instance.MuteAllSounds();
                _isSoundMuted = true;
                if (_audioButtonImage != null)
                {
                    _audioButtonImage.sprite = UnmuteAudioSprite;
                }
            }
        }

        public void OnSkipClicked()
        {
            controlls.interactable = false;
            LevelManager.Instance.LoadNextLevel();
        }
    }
}