using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance;
        
        private const string TIME_FORMAT = "ss'.'ff";

        [SerializeField]
        private TextMeshProUGUI txtTimer;
        
        [SerializeField]
        private TextMeshProUGUI txtNewBestTime;
        
        [SerializeField]
        private Color colorNewBestTime;
        
        [SerializeField]
        private Color colorNoBestTime;

        [SerializeField]
        private List<int> blackListLevel = new List<int>() {0, 1};

        public IReadOnlyDictionary<int, float> BestTime => bestTime;
        private Dictionary<int, float> bestTime;

        private TimeSpan _timeSpan;
        private bool _count;
        

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ResetUi();

            bestTime = new Dictionary<int, float>();

            GameManager.Instance.OnGameStateChange += OnGameStateChange;
        }

        private void ResetUi()
        {
            bool isOnBlackList = blackListLevel.Contains(LevelManager.Instance.CurLevel);
            txtTimer.transform.gameObject.SetActive(!isOnBlackList);
            txtTimer.text = "00.00";
            _count = true;
            txtNewBestTime.alpha = 0; //hide best time
        }

        private void OnGameStateChange(GameState obj)
        {
            if (obj == GameState.OnEndpoint)
            {
                float curTime = Time.timeSinceLevelLoad;
                _count = false;
                CheckForNewBestTime(curTime);
            }
            if (obj == GameState.LevelComplete)
            {
                txtNewBestTime.DOFade(0, 0.2f);
            }
            else if (obj == GameState.Playing)
            {
                ResetUi();
            }
        }

        private void CheckForNewBestTime(float curTime)
        {
            int curLevel = LevelManager.Instance.CurLevel;
            bool newBestTime = false;
            if (bestTime.TryGetValue(curLevel, out float curBestTime))
            {
                Debug.Log($"best time for level {curLevel} was {curBestTime} now {curTime} best? {curBestTime > curTime}");
                if (curBestTime > curTime)
                {
                    bestTime[curLevel] = curTime;
                    newBestTime = true;
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(curBestTime);
                    txtNewBestTime.SetText(timeSpan.ToString(TIME_FORMAT));
                }
            }
            else
            {
                Debug.Log("No besttime for level " + curLevel);
                bestTime.Add(curLevel, curTime);
                newBestTime = true;
            }

            if (newBestTime)
            {
                txtNewBestTime.color = colorNewBestTime;
                txtNewBestTime.SetText("new best time");
            }
            else
            {
                txtNewBestTime.color = colorNoBestTime;
            }
            
            StartNewBestTimeAnimation();
        }

        private void StartNewBestTimeAnimation()
        {
            txtNewBestTime.DOFade(100, 0.2f).SetEase(Ease.InOutSine);
            txtNewBestTime.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.4f).SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
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
