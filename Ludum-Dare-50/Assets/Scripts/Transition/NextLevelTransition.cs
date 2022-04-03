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
    
    [SerializeField]
    private float durationOpen = 0.1f;
    
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

    public void DoTransition(TransitionFace doTransition, TweenCallback onComplete = null)
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
                FaceOpen(onComplete);
                break;
            case TransitionFace.Close: 
                FaceClose(onComplete);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void SetElementsActive(bool status)
    {
        right.gameObject.SetActive(status);
        left.gameObject.SetActive(status);
    }
    
    private void FaceClose(TweenCallback onComplete = null)
    {
        SetElementsActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(right.DOLocalMoveX(endValue: 0, duration: duration).From(_startPosRight).SetEase(Ease.OutSine));
        sq.Join(left.DOLocalMoveX(endValue: 0, duration: duration).From(_startPosLeft).SetEase(Ease.OutSine));
        sq.OnComplete(() => Completed(TransitionFace.Close,onComplete));
    }
    
    private void FaceOpen(TweenCallback onComplete = null)
    {
        SetElementsActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(right.DOLocalMoveX(endValue: _startPosRight, duration: durationOpen).From(0).SetEase(Ease.InSine));
        sq.Join(left.DOLocalMoveX(endValue: _startPosLeft, duration: durationOpen).From(0).SetEase(Ease.InSine));
        sq.OnComplete(() =>
        {
            Completed(TransitionFace.Open, onComplete);
            SetElementsActive(false);
        });
    }

    private void Completed(TransitionFace transitionFace, TweenCallback onComplete = null)
    {
        onComplete?.Invoke();
        _face = TransitionFace.Idle;
    }
}

