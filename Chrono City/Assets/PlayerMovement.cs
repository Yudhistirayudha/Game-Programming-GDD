using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rbody;

    //Basic Movement
    private float horizontalMove;
    //Flip character
    private bool isFacingRight = true;

    //Dash Feature
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private float dashingPower = 24f;


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 11f;
    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal");
       
        if (isDashing)
            return;

        if (Input.GetButtonDown("Jump") && isGrounded())
            _rbody.velocity = new Vector2(_rbody.velocity.x, jumpForce);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

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
