using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float Speed = 5f;

    [SerializeField]
    private float JumpForce = 12f;

    [SerializeField]
    private float JumpBuffer = 0.1f;

    [FormerlySerializedAs("MAXFallSpeed")]
    [SerializeField]
    private float MaxFallSpeed = -25f;

    [FormerlySerializedAs("groudCheck")]
    [SerializeField]
    private Transform GroundCheck;

    [FormerlySerializedAs("checkRadius")]
    [SerializeField]
    private float CheckRadius;

    [FormerlySerializedAs("whatIsGround")]
    [SerializeField]
    private LayerMask WhatIsGround;

    [SerializeField]
    private Rigidbody2D MyRigidbody2D;

    private readonly Collider2D[] _overlapResults = new Collider2D[2]; //can only detect 2 collisions

    private bool _doJump;

    private bool _isGrounded;

    private float _lastJumpPressed;

    private float _moveInput;

    private Animator _playerMovementAnimator;

    private bool _canMove = true;

    
    private SpriteRenderer[] _allSpriteRenderers;
    
    private bool HasBufferedJump => _lastJumpPressed + JumpBuffer > Time.time;

    private Action<GameState> _myOnGameStateChangeEvent;
    private bool _isFacingRight;

    private void Awake()
    {
        _doJump = false;
        _lastJumpPressed = float.MinValue;
        _playerMovementAnimator = GetComponentInChildren<Animator>();
        _allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _myOnGameStateChangeEvent = OnGameStateChange;
        GameManager.Instance.OnGameStateChange += _myOnGameStateChangeEvent;
    }

    private void OnGameStateChange(GameState state)
    {
        if (state != GameState.Dead)
        {
            return;
        }

        StopMovement();
        
        DeathAnimation();
        
        //remove my event! fix with reload scenes
        GameManager.Instance.OnGameStateChange -= _myOnGameStateChangeEvent;
        GameManager.Instance.UpdateGameState(GameState.Reload);
    }

    private void DeathAnimation()
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(gameObject.transform.DORotate(endValue: new Vector3(x: 0, y: 0, z: 180 * (_isFacingRight ? -1 : 1)),
            duration: 0.5f));
        sq.Join(gameObject.transform.DOMoveY(endValue: transform.position.y - 10f, duration: 1f));
        foreach (SpriteRenderer sprite in _allSpriteRenderers)
        {
            if (sprite == null)
            {
                continue;
            }

            sq.Join(sprite.DOFade(endValue: 0, duration: 1f));
        }
    }

    private void Update()
    {
        if (!_canMove)
        {
            return;
        }
        
        DoJump(Input.GetButtonDown("Jump"));

        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        SetFacingDirectionAnimation(horizontalAxis);
        SetIsRunningAnimation(horizontalAxis);

        if (_isGrounded && (_doJump || HasBufferedJump))
        {
            MyRigidbody2D.velocity = Vector2.up * JumpForce;
            _isGrounded = false;
        }
    }

    private void DoJump(bool jumpButtonPressed)
    {
        if (jumpButtonPressed)
        {
            _lastJumpPressed = Time.time;
            _doJump = true;
        }
        else
        {
            _doJump = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove)
        {
            return;
        }
        _isGrounded = Physics2D.OverlapCircleNonAlloc(point: GroundCheck.position, radius: CheckRadius, results: _overlapResults, layerMask: WhatIsGround) > 0;
        _moveInput = Input.GetAxisRaw("Horizontal");
        MyRigidbody2D.velocity = new Vector2(x: _moveInput * Speed, y: Mathf.Max(a: MaxFallSpeed, b: MyRigidbody2D.velocity.y));
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center: GroundCheck.position, radius: CheckRadius);
    }

    private void SetFacingDirectionAnimation(float horizontalAxis)
    {
        bool oldIsFacingRight = _playerMovementAnimator.GetBool("IsFacingRight");
        if (!oldIsFacingRight && horizontalAxis == 0)
        {
            return;
        }
        
        _isFacingRight = horizontalAxis >= 0;
        _playerMovementAnimator.SetBool(name: "IsFacingRight", value: _isFacingRight);
    }

    private void SetIsRunningAnimation(float horizontalAxis)
    {
        bool isHorizontalButtonPressed = horizontalAxis != 0f;
        _playerMovementAnimator.SetBool(name: "IsRunning", value: isHorizontalButtonPressed);
    }

    public void OnFoundExit()
    {
        StopMovement();
        enabled = false;
    }

    private void StopMovement()
    {
        _canMove = false;
        if(MyRigidbody2D != null)
        {
            MyRigidbody2D.bodyType = RigidbodyType2D.Static;
            // MyRigidbody2D.simulated = false;
            // MyRigidbody2D.velocity = Vector3.zero;
            // MyRigidbody2D.gravityScale = 0f;
        }
    }
}