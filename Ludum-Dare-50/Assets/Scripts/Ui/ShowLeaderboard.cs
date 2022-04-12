using System;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class ShowLeaderboard : MonoBehaviour
    {
        [SerializeField]
        private GameObject rowPrefab;

        private void Start()
        {
            foreach (KeyValuePair<int, float> bestTime in TimerManager.Instance.BestTime)
            {
                GameObject row = Instantiate(rowPrefab, this.transform, true);
                TextMeshProUGUI tm = row.GetComponent<TextMeshProUGUI>();
                tm.SetText($"Level{bestTime.Key}: {TimerManager.ParseTimeToString(bestTime.Value)}");
            }
        }
    }
}
