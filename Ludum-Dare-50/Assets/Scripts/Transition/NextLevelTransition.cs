using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NextLevelTransition : MonoBehaviour
{
    [SerializeField]
    private RectTransform right;

    [SerializeField]
    private RectTransform left;
    
    [SerializeField]
    private float duration = 0.3f;

    public enum TransitionFace
    {
        Idle,
        Running,
        Close,
        Open
    }
    public bool IsRunning => _face == TransitionFace.Running;
    
    private TransitionFace _face;
    private float _startPosRight;
    private float _startPosLeft;

    private void Start()
    {
        _startPosRight = right.localPosition.x;
        _startPosLeft = left.localPosition.x;
        _face = TransitionFace.Idle; 
    }

    public void DoTransition(TransitionFace doTransition)
    {
        if(IsRunning)
        {
            return;
        }
        _face = TransitionFace.Running; 

        switch (doTransition)
        {
            case TransitionFace.Idle: break;
            case TransitionFace.Running: break;
            case TransitionFace.Open:
                FaceOpen();
                break;
            case TransitionFace.Close: 
                FaceClose();
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void SetElementsActive(bool status)
    {
        right.gameObject.SetActive(status);
        left.gameObject.SetActive(status);
    }
    
    private void FaceClose()
    {
        SetElementsActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(right.DOLocalMoveX(endValue: 0, duration: duration).From(_startPosRight).SetEase(Ease.OutSine));
        sq.Join(left.DOLocalMoveX(endValue: 0, duration: duration).From(_startPosLeft).SetEase(Ease.OutSine));
        sq.OnComplete(() => Completed(TransitionFace.Close));
    }
    
    private void FaceOpen()
    {
        SetElementsActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(right.DOLocalMoveX(endValue: _startPosRight, duration: duration).From(0).SetEase(Ease.OutSine));
        sq.Join(left.DOLocalMoveX(endValue: _startPosLeft, duration: duration).From(0).SetEase(Ease.OutSine));
        sq.OnComplete(() =>
        {
            SetElementsActive(false);
            Completed(TransitionFace.Open);
        });
    }

    private void Completed(TransitionFace transitionFace)
    {
        _face = TransitionFace.Idle;
    }
}

