using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    private Action<GameState> _onEndpointReached;

    private GameObject _player;

    private bool _isPlayerNear;

    private bool _wasTriggert;


    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _onEndpointReached = (GameState state) =>
        {
            if (state != GameState.OnEndpoint) return;
            StartCoroutine(StartEndAnimation(_player));
        };
        GameManager.Instance.OnGameStateChange += _onEndpointReached;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= _onEndpointReached;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }
        if(_wasTriggert)
            return;
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
        SoundManager.Instance.StopSound(SoundManager.Sounds.Alarm, fade: true);
        GameManager.Instance.UpdateGameState(GameState.LevelComplete);
    }

    private void SpawnFist(Transform playerTransform, TweenCallback onComplete = null)
    {
        GameObject fistObj = Instantiate(fist);
        GameObject snoozeObj = Instantiate(snooze);
        TextAnimator snooteAnimation = snoozeObj.GetComponent<TextAnimator>();

        Transform fistTransform = fistObj.transform;
        Transform snoozeTransform = snoozeObj.transform;

        //fist
        Vector3 playerPos = playerTransform.position;
        Vector3 endPos = new Vector3(x: playerPos.x, y: playerPos.y + 1.4f);
        Vector3 startPos = new Vector3(x: playerPos.x, y: playerPos.y + 10, z: 0);
        fistTransform.position = startPos;

        //snooze
        snoozeTransform.position = new Vector3(endPos.x - 2.2f, endPos.y);
        snooteAnimation.Init();

        //play music
        SoundManager.Instance.StopSound(SoundManager.Sounds.AlarmTicking);
        SoundManager.Instance.PlaySound(SoundManager.Sounds.Alarm);

        Sequence sp = DOTween.Sequence();
        sp.SetDelay(0.5f);
        sp.Append(fistTransform.DOMove(endValue: endPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.Append(playerTransform.DOShakeScale(shakeSpeed));
        sp.Join(snooteAnimation.GetAnimation());
        sp.Join(SoundManager.Instance.StopSoundFadeOutAndPitch(SoundManager.Sounds.Alarm));
        sp.Append(fistTransform.DOMove(endValue: startPos, duration: fistSpeed).SetEase(Ease.Flash));
        sp.OnComplete(onComplete);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceTicking);
    }

    private void Update()
    {
        if (Vector2.Distance(_player.transform.position, transform.position) <= distanceTicking)
        {
            if (!_isPlayerNear)
            {
                Debug.Log("PLAY!!!");
                SoundManager.Instance.PlaySound(SoundManager.Sounds.AlarmTicking, fade: true);
                _isPlayerNear = true;
            }
        }
        else
        {
            if (_isPlayerNear)
            {
                Debug.Log("Stop!!!");
                SoundManager.Instance.StopSound(SoundManager.Sounds.AlarmTicking, true);
            }

            _isPlayerNear = false;
        }

        //just for testing
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("spawn");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            SpawnFist(player.transform);
        }
    }
}