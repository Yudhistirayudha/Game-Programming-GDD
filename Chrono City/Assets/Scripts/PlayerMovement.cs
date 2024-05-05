using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalMove;
    private bool isFacingRight = true;

    private Rigidbody2D rb;

    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();  
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CheckMovementDir()
    {
        if (isFacingRight && horizontalMove < 0)
        {
            Flip();
        }

        else if (isFacingRight && horizontalMove > 0)
        {
            Flip();
        }
    }

    private void CheckInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
