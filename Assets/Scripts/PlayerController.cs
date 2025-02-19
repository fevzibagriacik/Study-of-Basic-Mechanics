using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Move Settings")]
    public float moveSpeed = 1500;
    private float moveInput;

    [Header("Jump Settings")]
    public float jumpForce;
    public float checkRadius = 0.3f;
    public float wallCheckRadius = 0.3f;
    public float wallJumpForce = 10f;
    public float wallSlidingSpeed = 0.2f;
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

    [Header("Dash Settings")]
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
    public float direction;
    [SerializeField] bool canDash = true;
    [SerializeField] bool isDashing;
    [SerializeField] private TrailRenderer tr;

    [Header("Shoot Settings")]
    private Vector3 mousePos;
    private Camera mainCam;
    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    public float timer;
    public float shootCooldown;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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

        if (!canFire)
        {
            timer += Time.deltaTime;
            if(timer > shootCooldown)
            {
                canFire = true;
                timer = 0f;
            }
        }

        IsGrounded();

        Move();

        Flip();

        Jump();

        DoubleJump();

        WallSlide();

        WallJump();

        if(Input.GetKeyDown(KeyCode.F) && canDash)
        {
            StartCoroutine(Dash());
        }

        //RotateBulletTransform();

        Shoot();
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

    public void Flip()
    {
        if(moveInput > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            sr.flipX = true;
        }
        else if(moveInput < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            sr.flipX = false;
        }
    }

    public void RotateBulletTransform()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    public void Shoot()
    {
        if(canFire && Input.GetMouseButtonDown(0))
        {
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
        }
    }
}
