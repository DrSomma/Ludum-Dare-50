using System;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float Speed = 5f;

        [SerializeField]
        private float JumpForce = 12f;

        [SerializeField]
        private float JumpVelocityFalloff = 8;
        
        [SerializeField]
        private float FallMultiplier = 7;
        
        [SerializeField]
        private float JumpBuffer = 0.1f;
        
        [SerializeField]
        private float CoyoteTime = 0.4f;

        [SerializeField]
        private int MaxJumpCount = 1;
        
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

        [SerializeField]
        private PlayerParticleController playerParticles;
        
        private readonly Collider2D[] _overlapResults = new Collider2D[2]; //can only detect 2 collisions

        public bool OnGround => _isGrounded;
        
        private bool _isGrounded;

        private float _lastJumpPressed;
        
        private float _lastTimeOnGround;
        
        private float _moveInput;

        private int _jumpCount;

        private Animator _playerMovementAnimator;

        private bool _canMove = true;

        private SpriteRenderer[] _allSpriteRenderers;
    
        private bool HasBufferedJump => _lastJumpPressed + JumpBuffer > Time.time;
        private bool IsInAir => !_isGrounded;

        private Action<GameState> _myOnGameStateChangeEvent;
        private bool _isFacingRight;
      

        private void Awake()
        {
            _lastJumpPressed = float.MinValue;
            _playerMovementAnimator = GetComponentInChildren<Animator>();
            _allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            _jumpCount = MaxJumpCount;
            _canMove = GameManager.Instance.CurrentState == GameState.Playing;
            _isGrounded = IsGrounded();
            _myOnGameStateChangeEvent = OnGameStateChange;
            GameManager.Instance.OnGameStateChange += _myOnGameStateChangeEvent;
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    _canMove = true;
                    break;
                case GameState.Dead:
                    OnGameStateDeath();
                    break;
            }
        }

        private void OnGameStateDeath()
        {
            StopMovement(true);
        
            DeathAnimation();
        
            GameManager.Instance.UpdateGameState(GameState.Reload);
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChange -= _myOnGameStateChangeEvent;
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
            
            float horizontalAxis = Input.GetAxisRaw("Horizontal");

            SetFacingDirectionAnimation(horizontalAxis);
            SetIsRunningAnimation(horizontalAxis);

            if ((IsInAir && MyRigidbody2D.velocity.y < JumpVelocityFalloff) || MyRigidbody2D.velocity.y > 0 && !Input.GetButton("Jump"))
                MyRigidbody2D.velocity += Vector2.up * Physics.gravity.y * FallMultiplier * Time.deltaTime;
            
            if (CheckForJumpInput(Input.GetButtonDown("Jump")))
            {
                DoJump();
            }
        }

        private void DoJump()
        {
            _jumpCount--;
            
            //animation
            Sequence sp = DOTween.Sequence();
            sp.Append(transform.DOScale(new Vector3(1, 0.5f, 1), 0.02f).From(Vector3.one).OnComplete(
                () =>
                {
                    MyRigidbody2D.velocity = Vector2.up * JumpForce;
                    _lastTimeOnGround = Time.time;
                }));
            sp.Append(transform.DOScale(new Vector3(0.5f, 1, 1), 0.2f));
            sp.Append(transform.DOScale(new Vector3(1, 1, 1), 0.5f));

            //particles
            playerParticles.SpawnDustParticels();
            
            //sound
            SoundManager.Instance.PlaySound(SoundManager.Sounds.Jump);
        }

        private bool CheckForJumpInput(bool inputButtonPressed)
        {
            if (inputButtonPressed)
            {
                _lastJumpPressed = Time.time;
            }
            
            bool isJumpPressed = (inputButtonPressed || HasBufferedJump);
          
            bool coyote =  _lastTimeOnGround + CoyoteTime > Time.time;
            bool hasJumpLeft = (_jumpCount > 0);
            bool canJump = coyote || _isGrounded;
            bool doJump = hasJumpLeft && isJumpPressed && canJump;
            
            if (doJump)
            {
                _lastJumpPressed = float.MinValue;
            }
            
            return doJump;
        }

        private void FixedUpdate()
        {
            if (IsGrounded())
            {
                //not landet in this frame => call OnLande once
                if(IsInAir) 
                    OnLand();
            }
            else
            {
                //in this frame still on ground
                if (_isGrounded) _lastTimeOnGround = Time.time; 
                _isGrounded = false;
            }
        
            if (!_canMove)
            {
                return;
            }
            _moveInput = Input.GetAxisRaw("Horizontal");
            MyRigidbody2D.velocity = new Vector2(x: _moveInput * Speed, y: Mathf.Max(a: MaxFallSpeed, b: MyRigidbody2D.velocity.y));
        }

        private void OnLand()
        {
            _isGrounded = true;
            _jumpCount = MaxJumpCount;
            playerParticles.SpawnDustParticels();
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircleNonAlloc(point: GroundCheck.position, radius: CheckRadius, results: _overlapResults, layerMask: WhatIsGround) > 0;
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
            bool nowIsFacingRight = horizontalAxis > 0;
            bool isFacingChanged = (oldIsFacingRight ^ nowIsFacingRight) && horizontalAxis != 0;
            if (!isFacingChanged)
            {
                return;
            }
        
            DoFlip(nowIsFacingRight);
        }

        private void DoFlip(bool nowIsFacingRight)
        {
            playerParticles.SpawnDustParticels();
            _isFacingRight = nowIsFacingRight;
            _playerMovementAnimator.SetBool(name: "IsFacingRight", value: _isFacingRight);
        }

        private void SetIsRunningAnimation(float horizontalAxis)
        {
            bool isHorizontalButtonPressed = horizontalAxis != 0f;
            _playerMovementAnimator.SetBool(name: "IsRunning", value: isHorizontalButtonPressed);
        }

        public void OnFoundExit()
        {
            StopMovement(false);
        }

        private void StopMovement(bool stopPhysics)
        {
            _canMove = false;
            if(MyRigidbody2D != null)
            {
                if (stopPhysics)
                {
                    MyRigidbody2D.bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    MyRigidbody2D.velocity = Vector3.zero;
                }
            }
        }
    }
}