using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rbody;
    private Animator _anim;
    private RaycastHit2D _rcast;

    //Basic Movement
    private float horizontalMove;
    private bool jumpMove;
    private bool isTouchingWall = false;

    //Flip character
    private bool isFacingRight = true;

    //Dash Feature
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private float dashingPower = 24f;

    public LayerMask wallLayer;


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 11f;
    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        jumpMove = Input.GetKeyDown(KeyCode.W);

        if (isDashing)
            return;

        if (jumpMove && isGrounded())
         _rbody.velocity = new Vector2(_rbody.velocity.x, jumpForce);

        RaycastHit2D wallHitLeft = Physics2D.Raycast(transform.position, Vector2.left, 4.5f, wallLayer);
        RaycastHit2D wallHitRight = Physics2D.Raycast(transform.position, Vector2.right, 4.5f, wallLayer);

        if (wallHitLeft.collider != null || wallHitRight.collider != null)
            isTouchingWall = true;
        else
            isTouchingWall = false;

        if (jumpMove && isTouchingWall && !isGrounded())
        {
            _rbody.velocity = new Vector2(_rbody.velocity.x, jumpForce);
            if (wallHitLeft.collider != null)
                _rbody.AddForce(new Vector2(jumpForce, 0)); //Push to the right

            else if (wallHitRight.collider != null)
                _rbody.AddForce(new Vector2(-jumpForce, 0)); //Push to the left
        }



        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        //Animator param
        _anim.SetBool("Run", horizontalMove != 0);

         Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        _rbody.velocity = new Vector2(horizontalMove * speed, _rbody.velocity.y);
    }

    //Method to Flip Character
    void Flip()
    {
        if (isFacingRight && horizontalMove < 0f || !isFacingRight && horizontalMove > 0f) 
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Method to check the ground
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

   
        
 

    //Method to initiate dash
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = _rbody.gravityScale;
        _rbody.gravityScale = 0f;
        _rbody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        //Code so player can spam dash
        yield return new WaitForSeconds(dashingTime);
        _rbody.gravityScale = originalGravity;
        isDashing = false;

        //Cooldown time
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
