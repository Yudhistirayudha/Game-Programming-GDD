using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Rigidbody2D _rbody;
    public Animator _animator;

    bool isFacingRight = true;
    bool isRun;

    // Movement Feature
    [Header("Movement")]
    public float speed = 5f;
    float horizontalMovement;

    [Header("Dash")]
    public float dashSpeed = 50f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool canDash = true;
    bool isDashing;
    TrailRenderer _trailRenderer;

    // Jump & Double Jump Feature 
    [Header("Jumping")]
    public float jumpPower = 11f;
    public int maxJumps = 1;
    int jumpsRemaining;

    // Ground Check Feature
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.7f, 0.04f);
    public LayerMask groundLayer;
    bool isGrounded;

    // Faster Fall Feature
    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMult = 2f;

    // Wall Check Feature
    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.04f, 1.15f);
    public LayerMask wallLayer;

    // Wall Slide Feature
    [Header("WallMovement")]
    public float wallSlideSpeed = 3f;
    bool isWallSliding;

    // Wall Jump Feature
    bool isWallJumping;
    float wallJumpDir;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(3f, 11f);

    // Start is called before the first frame update
    void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            _rbody.velocity = new Vector2(horizontalMovement * speed, _rbody.velocity.y);
            if (horizontalMovement > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (horizontalMovement < 0 && isFacingRight)
            {
                Flip();
            }
        }

        _animator.SetFloat("yVelocity", _rbody.velocity.y);
        _animator.SetFloat("magnitude", _rbody.velocity.magnitude);
        _animator.SetBool("isWallSliding", isWallSliding);

    }

    private void Flip() // Flip the character when facing right or left
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void Move(InputAction.CallbackContext context) // Input for Movement || Movement Method
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context) // Input for Dash
    {
        if (context.performed && canDash)
            StartCoroutine(Dash());
    }

    public void Jump(InputAction.CallbackContext context) // Input for Jump || Jump Method & Double Jump Method
    {
        if (jumpsRemaining > 0)
            if (context.performed)
            {
                _rbody.velocity = new Vector2(_rbody.velocity.x, jumpPower);
                jumpsRemaining--;
                _animator.SetTrigger("jump");
            }
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            _rbody.velocity = new Vector2 (wallJumpDir * wallJumpPower.x, wallJumpPower.y); // Jump Away From Wall
            wallJumpTimer = 0;
            _animator.SetTrigger("jump");

            // Force Flip
            if (transform.localScale.x != wallJumpDir)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0f, 180f, 0f);
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); // Wall Jump = 0.5f -- Jump Again = 0.06f
        }
    }

    private void GroundCheck() // Ground Check Method
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
    }

    private bool WallCheck() // Wall Check Method
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Gravity() // Faster Fall Method
    {
        if (_rbody.velocity.y < 0)
        {
            _rbody.gravityScale = baseGravity * fallSpeedMult;
            _rbody.velocity = new Vector2(_rbody.velocity.x, Mathf.Max(_rbody.velocity.y, -maxFallSpeed));
        }
        else
        {
            _rbody.gravityScale = baseGravity;
        } 
            
    }

    private void WallSlide() // Wall Slide Method
    {
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            _rbody.velocity = new Vector2(_rbody.velocity.x, Mathf.Max(_rbody.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDir = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        _trailRenderer.emitting = true;
        float dashDir = isFacingRight ? 1f : -1f;

        _rbody.velocity = new Vector2(dashDir * dashSpeed, _rbody.velocity.y); //Dash Movement

        yield return new WaitForSeconds(dashDuration);

        _rbody.velocity = new Vector2(0f, _rbody.velocity.y); //Reset horizontal velocity

        isDashing = false;
        _trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void OnDrawGizmos() // Drawing Transparent Cube (Deleted When Game is Builded)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
