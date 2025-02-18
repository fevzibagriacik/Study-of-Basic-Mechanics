using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
    public float direction;

    [SerializeField] int jumpCount = 0;

    [SerializeField] bool isGrounded;
    [SerializeField] bool isWallSliding;
    [SerializeField] bool isWallJumping;
    [SerializeField] bool canDash = true;
    [SerializeField] bool isDashing;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private TrailRenderer tr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rb.velocity.x < 0f)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        if (isDashing)
        {
            return;
        }

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

        if(Input.GetKeyDown(KeyCode.F) && canDash)
        {
            StartCoroutine(Dash());
        }
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

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(direction * dashPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
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
