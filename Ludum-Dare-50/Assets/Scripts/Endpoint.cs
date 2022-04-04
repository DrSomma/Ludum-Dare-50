using System;
using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;

public class Endpoint : MonoBehaviour
{
    [SerializeField]
    private GameObject fist;

    [SerializeField]
    private GameObject snooze;

    [SerializeField]
    private float fistSpeed = 0.25f;

    [SerializeField]
    private float shakeSpeed = 1f;

    [SerializeField]
    private float distanceTicking = 4f;

    private bool _isPlayerNear;

    private Action<GameState> _onEndpointReached;

    private GameObject _player;
    private Animator _playerMovementAnimator;

    private bool _wasTriggert;


    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerMovementAnimator = _player.GetComponentInChildren<Animator>();

        _onEndpointReached = state =>
        {
            if (state != GameState.OnEndpoint)
            {
                return;
            }

            StartCoroutine(StartEndAnimation(_player));
        };
        GameManager.Instance.OnGameStateChange += _onEndpointReached;
    }

    private void Update()
    {
        DoPlayerNearTrigger();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= _onEndpointReached;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center: transform.position, radius: distanceTicking);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (_wasTriggert)
        {
            return;
        }

        _wasTriggert = true;

        Debug.Log("Exit!");

        GameManager.Instance.UpdateGameState(GameState.OnEndpoint);
    }

    private IEnumerator StartEndAnimation(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.OnFoundExit();
        Debug.Log("WaitUntil OnGround!");
        yield return new WaitUntil(() => playerMovement.OnGround);
        Debug.Log("WaitUntil now OnGround!");
        SpawnFist(playerTransform: player.transform, onComplete: AnimationIsDone);
        yield return null;
    }

    private void AnimationIsDone()
    {
        SoundManager.Instance.StopSound(soundEnum: SoundManager.Sounds.Alarm, fade: true);
        _playerMovementAnimator.SetBool(name: "IsRinging", value: true);
        GameManager.Instance.UpdateGameState(GameState.LevelComplete);
    }

    private void SpawnFist(Transform playerTransform, TweenCallback onComplete = null)
    {
        GameObject fistObj = Instantiate(fist);
        GameObject snoozeObj = Instantiate(snooze);
        TextAnimator snoozeAnimation = snoozeObj.GetComponent<TextAnimator>();

        Transform fistTransform = fistObj.transform;
        Transform snoozeTransform = snoozeObj.transform;

        //fist
        Vector3 playerPos = playerTransform.position;
        Vector3 endPos = new Vector3(x: playerPos.x, y: playerPos.y + 1.4f);
        Vector3 startPos = new Vector3(x: playerPos.x, y: playerPos.y + 10, z: 0);
        fistTransform.position = startPos;

        //snooze
        snoozeTransform.position = new Vector3(x: endPos.x - 2.2f, y: endPos.y);
        snoozeAnimation.Init();

        //play music
        SoundManager.Instance.StopSound(SoundManager.Sounds.AlarmTicking);
        SoundManager.Instance.PlaySound(SoundManager.Sounds.Alarm);

        Sequence sp = DOTween.Sequence();
        sp.SetDelay(0.5f);
        sp.Append(fistTransform.DOMove(endValue: endPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.Append(GetAnimationSequenceOnFistHit(playerTransform: playerTransform, snoozeAnimation: snoozeAnimation));
        sp.Append(GetAnimationSequenceOnFistUp(fistTransform: fistTransform, startPos: startPos, snoozeAnimation: snoozeAnimation));
        sp.OnComplete(onComplete);
    }

    private Tween GetAnimationSequenceOnFistUp(Transform fistTransform, Vector3 startPos, TextAnimator snoozeAnimation)
    {
        Sequence sp = DOTween.Sequence();
        sp.Append(fistTransform.DOMove(endValue: startPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.Join(snoozeAnimation.GetAnimation(true));
        return sp;
    }

    private Tween GetAnimationSequenceOnFistHit(Transform playerTransform, TextAnimator snoozeAnimation)
    {
        Sequence sp = DOTween.Sequence();
        sp.Append(playerTransform.DOShakeScale(shakeSpeed));
        sp.Join(snoozeAnimation.GetAnimation(false));
        sp.Join(SoundManager.Instance.StopSoundFadeOutAndPitch(SoundManager.Sounds.Alarm));
        sp.OnPlay(() => { playerTransform.GetComponent<PlayerLifeController>().HitPlayer(); });
        return sp;
    }

    private void DoPlayerNearTrigger()
    {
        if (IsPlayerNear())
        {
            if (!_isPlayerNear)
            {
                Debug.Log("PLAY!!!");
                OnPlayerNearTriggerEnter();
                _isPlayerNear = true;
            }
        }
        else
        {
            if (_isPlayerNear)
            {
                Debug.Log("Stop!!!");
                OnPlayerNearTriggerExit();
            }

            _isPlayerNear = false;
        }
    }

    private void OnPlayerNearTriggerExit()
    {
        SoundManager.Instance.StopSound(soundEnum: SoundManager.Sounds.AlarmTicking, fade: true);
        _playerMovementAnimator.SetBool(name: "IsTickingFaster", value: false);
    }

    private void OnPlayerNearTriggerEnter()
    {
        SoundManager.Instance.PlaySound(soundEnum: SoundManager.Sounds.AlarmTicking, fade: true);
        _playerMovementAnimator.SetBool(name: "IsTickingFaster", value: true);
    }

    private bool IsPlayerNear()
    {
        return Vector2.Distance(a: _player.transform.position, b: transform.position) <= distanceTicking;
    }
}