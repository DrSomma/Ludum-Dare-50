using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float endPosX;
    private Vector3 EndPos => new Vector3(endPosX,transform.position.y);
    
    private void Start()
    {
        transform.DOMove(EndPos, speed, false).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(EndPos,0.4f);
    }
}
