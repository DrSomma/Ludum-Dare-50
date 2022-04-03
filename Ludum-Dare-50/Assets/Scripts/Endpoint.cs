using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Endpoint : MonoBehaviour
{
    [SerializeField]
    private GameObject fist;
    
    [SerializeField]
    private float fistSpeed = 0.25f;
    [SerializeField]
    private float shakeSpeed = 1f;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Exit!");
        PlayerMovement playerMovement = col.GetComponent<PlayerMovement>();
        playerMovement.OnFoundExit();
        SpawnFist(col.transform, AnimationIsDone);
        
    }

    private void AnimationIsDone()
    {
        GameManager.Instance.UpdateGameState(GameState.LevelComplete);
    }

    private void SpawnFist(Transform playerTransform, TweenCallback onComplete = null)
    {
        GameObject fistObj = Instantiate(fist);
        Transform fistTransform = fistObj.transform;
        
        Vector3 playerPos = playerTransform.position;
        Vector3 endPos = new Vector3(x: playerPos.x, y: playerPos.y + 1);
        Vector3 startPos = new Vector3(x: playerPos.x, y: playerPos.y+10, z: 0);
        fistTransform.position = startPos;

        Sequence sp = DOTween.Sequence();
        sp.Append(fistTransform.DOMove(endValue: endPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.Append(playerTransform.DOShakeScale(shakeSpeed));
        sp.Append(fistTransform.DOMove(endValue: startPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.OnComplete(onComplete);
    }

    //just for testing
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         Debug.Log("spawn");
    //         GameObject player = GameObject.FindGameObjectWithTag("Player");
    //         SpawnFist(player.transform);
    //     }
    // }
}
