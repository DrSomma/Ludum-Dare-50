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

    private readonly Collider2D[] _overlapResults = new Collider2D[2]; //can only detect 2 collisions

    private bool _doJump;

    private bool _isGrounded;

    private float _lastJumpPressed;

    private float _moveInput;

    private Animator _playerMovementAnimator;

    private Rigidbody2D _rb;
    private bool HasBufferedJump => _lastJumpPressed + JumpBuffer > Time.time;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _doJump = false;
        _lastJumpPressed = float.MinValue;
        _playerMovementAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _lastJumpPressed = Time.time;
            _doJump = true;
        }
        else
        {
            _doJump = false;
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            SetFacingDirection();
        }

        if (_isGrounded && (_doJump || HasBufferedJump))
        {
            _rb.velocity = Vector2.up * JumpForce;
            _isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircleNonAlloc(point: GroundCheck.position, radius: CheckRadius, results: _overlapResults, layerMask: WhatIsGround) > 0;
        _moveInput = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(x: _moveInput * Speed, y: Mathf.Max(a: MaxFallSpeed, b: _rb.velocity.y));
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center: GroundCheck.position, radius: CheckRadius);
    }

    private void SetFacingDirection()
    {
        bool isFacingRight = Input.GetAxisRaw("Horizontal") > 0;
        _playerMovementAnimator.SetBool(name: "IsFacingRight", value: isFacingRight);
    }

    public void OnFoundExit()
    {
        _rb.bodyType = RigidbodyType2D.Static; //stop moving
        enabled = false;
    }
}