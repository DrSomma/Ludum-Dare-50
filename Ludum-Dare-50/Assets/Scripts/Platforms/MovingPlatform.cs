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
    private Vector2 endPosRelativ;
    private Vector3 EndPos => new Vector3(x: transform.position.x + endPosRelativ.x,y: transform.position.y + endPosRelativ.y);
    
    private void Start()
    {
        transform.DOMove(endValue: EndPos, duration: speed).SetEase(Ease.InOutSine).SetSpeedBased().SetLoops(loops: -1,loopType: LoopType.Yoyo);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        col.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        col.transform.SetParent(null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(center: EndPos,radius: 0.4f);
    }
}
