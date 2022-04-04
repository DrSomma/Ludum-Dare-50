using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLifeController : MonoBehaviour
{
    [SerializeField]
    private int playerLifeForThisLevel = 1;

    public event Action OnPlayerHit;
    
    private bool HasLifesLevt => _lifes == 0;
    
    private int _lifes;

    private void Start()
    {
        _lifes = playerLifeForThisLevel;
    }

    public void HitPlayer()
    {
        OnPlayerHit?.Invoke();
    }

    public void HurtPlayer()
    {
        HitPlayer();

        _lifes--;
        if (HasLifesLevt)
        {
            GameManager.Instance.UpdateGameState(GameState.Dead);
        }
    }

}
