using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _rbody.velocity = new Vector2(horizontalMovement * speed, _rbody.velocity.y);
        GroundCheck();
        Gravity();
        Flip();
        WallSlide();
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

    private void OnDrawGizmos() // Drawing Transparent Cube (Deleted When Game is Builded)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
