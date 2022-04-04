using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class TimerManager : MonoBehaviour
    {
        private const string TIME_FORMAT = "ss'.'ff";

        [SerializeField]
        private TextMeshProUGUI txtTimer;

        private TimeSpan _timeSpan;
        private bool _count;

        private void Start()
        {
            txtTimer.text = "00.00";
            _count = true;
            GameManager.Instance.OnGameStateChange += OnGameStateChange;
        }

        private void OnGameStateChange(GameState obj)
        {
            if (obj == GameState.OnEndpoint)
            {
                _count = false;
            }
            else if (obj == GameState.LevelComplete)
            {
                // txtTimer.text = "0.00";
            }
            else if (obj == GameState.Playing)
            {
                txtTimer.text = "00.00";
                _count = true;
            }
        }

        private void Update()
        {
            if(!_count)
                return;
            _timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
            txtTimer.text = _timeSpan.ToString(TIME_FORMAT);
        }

         
    }
}
