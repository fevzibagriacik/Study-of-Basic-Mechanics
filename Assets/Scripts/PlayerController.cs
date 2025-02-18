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
    public float wallCheckRadius = 0.3f;
    public float wallJumpForce = 10f;
    public float wallSlidingSpeed = 0.2f;
    public float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;

    [SerializeField] int jumpCount = 0;

    [SerializeField] bool isGrounded;
    [SerializeField] bool isWallSliding;
    [SerializeField] bool isWallJumping;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

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

        WallSlide();

        WallJump();
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

    public void WallSlide()
    {
        if(rb.velocity.y < 0 && !isGrounded && IsWalled())
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));

        }
        else
        {
            isWallSliding = false;
        }
    }

    public void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke("StopWallJumping");
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            wallJumpingCounter = 0f;
            Invoke("StopWallJumping", wallJumpingDuration);
        }
    }

    public void StopWallJumping()
    {
        isWallJumping = false;
    }

    public bool IsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        return isGrounded;
    }

    public bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }
}
