using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Rigidbody2D _rbody;

    bool isFacingRight = true;

    // Movement Feature
    [Header("Movement")]
    public float speed = 5f;
    float horizontalMovement;

    // Jump & Double Jump Feature 
    [Header("Jumping")]
    public float jumpPower = 11f;
    public int maxJumps = 1;
    int jumpsRemaining;

    // Ground Check Feature
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.82f, 0.07f);
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
    public Vector2 wallCheckSize = new Vector2(0.05f, 2.65f);
    public LayerMask wallLayer;

    // Wall Slide Feature
    [Header("WallMovement")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;

    // Wall Jump Feature
    bool isWallJumping;
    float wallJumpDir;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(3f, 10f);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            _rbody.velocity = new Vector2(horizontalMovement * speed, _rbody.velocity.y);
            Flip();
        }
    }

    private void Flip() // Flip the character when facing right or left
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void Move(InputAction.CallbackContext context) // Input for Movement || Movement Method
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) // Input for Jump || Jump Method & Double Jump Method
    {
        if (jumpsRemaining > 0)
            if (context.performed)
            {
                _rbody.velocity = new Vector2(_rbody.velocity.x, jumpPower);
                jumpsRemaining--;
            }
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            _rbody.velocity = new Vector2 (wallJumpDir * wallJumpPower.x, wallJumpPower.y); // Jump Away From Wall
            wallJumpTimer = 0;

            // Force Flip
            if(transform.localScale.x != wallJumpDir)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
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

    private void OnDrawGizmos() // Drawing Transparent Cube (Deleted When Game is Builded)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
