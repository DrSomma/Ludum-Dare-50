using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float jumpForce = 12f;

    [SerializeField]
    private float jumpBuffer = 0.1f;

    [SerializeField]
    private float maxFallSpeed = -25f;

    [SerializeField]
    private Transform groudCheck;

    [SerializeField]
    private float checkRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    public bool IsFacingRight { get; private set; }
    
    private bool _isGrounded;

    private float _lastJumpPressed;
    private bool HasBufferedJump => _lastJumpPressed + jumpBuffer > Time.time;
    private bool _doJumpe;

    private float _moveInput;

    private Rigidbody2D _rb;
    private readonly Collider2D[] _overlapResults = new Collider2D[2]; //can only detect 2 collisions

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _lastJumpPressed = Time.time;
            _doJumpe = true;
        }
        else
        {
            _doJumpe = false;
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            IsFacingRight = Input.GetAxisRaw("Horizontal") > 0;
        }

        if (_isGrounded && (_doJumpe || HasBufferedJump))
        {
            _rb.velocity = Vector2.up * jumpForce;
            _isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircleNonAlloc(point: groudCheck.position, radius: checkRadius, results: _overlapResults, layerMask: whatIsGround) > 0;
        _moveInput = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(x: _moveInput * speed, y: Mathf.Max(a: maxFallSpeed, b: _rb.velocity.y));
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center: groudCheck.position, radius: checkRadius);
    }

    public void OnFoundExit()
    {
        _rb.bodyType = RigidbodyType2D.Static; //stop moving
        this.enabled = false;
    }
}