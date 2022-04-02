using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    
    [SerializeField]
    private float strength = 20f;
    [SerializeField]
    private int loopCount = 3;
    
    private bool _isBreaking;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }

        Break();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
            Break();
        if (Input.GetKeyDown(KeyCode.H))
        {
            _isBreaking = false;
            Debug.Log("Kill: " + transform.DOKill(true));
            transform.rotation = Quaternion.identity;
        }
    }

    private void Break()
    {
        if (_isBreaking)
            return;
        _isBreaking = true;
        
        var sq = DOTween.Sequence();
        sq.Append(transform.DORotate(Vector3.forward * strength, speed, RotateMode.FastBeyond360));
        sq.Append(transform.DORotate(Vector3.back * strength, speed, RotateMode.FastBeyond360));
        sq.SetLoops(loopCount, LoopType.Yoyo);
        sq.OnComplete(DestroyMe);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
