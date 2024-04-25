using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rbody;

    private float horizontalMove;

    [SerializeField] private float speed = 10f;

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
        _rbody.velocity = new Vector2(horizontalMove * speed, _rbody.velocity.y);
    }
}
