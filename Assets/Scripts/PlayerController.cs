using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float moveSpeed;
    private float moveInput;
    public float jumpForce;
    public float checkRadius = 0.3f;

    [SerializeField] int jumpCount = 0;

    private bool isGrounded;

    public Transform groundCheck;

    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isGrounded)
        {
            jumpCount = 0;
        }

        IsGrounded();

        Move();

        Jump();

        DoubleJump();
    }

    public void Move()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed * Time.deltaTime, rb.velocity.y);
    }

    public void Jump()
    {
        if(isGrounded && (Input.GetKeyDown(KeyCode.Space)))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void DoubleJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && jumpCount < 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    public bool IsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        return isGrounded;
    }
}
