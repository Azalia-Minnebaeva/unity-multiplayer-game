using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController2D : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 18f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;

    private float moveX;
    private bool jumpRequested;
    private float originalScaleX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found!");
        }

        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        HandleFlip();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        Move();
        Jump();
    }

    void HandleInput()
    {
        moveX = 0f;

        if (Keyboard.current.aKey.isPressed)
            moveX = -1f;

        if (Keyboard.current.dKey.isPressed)
            moveX = 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (!jumpRequested) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpRequested = false;
    }

    void HandleFlip()
    {
        if (moveX > 0.01f)
        {
            transform.localScale = new Vector3(
                originalScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (moveX < -0.01f)
        {
            transform.localScale = new Vector3(
                -originalScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    void HandleAnimation()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(moveX));
        animator.SetBool("IsJumping", !IsGrounded());
    }

    bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck is not assigned!");
            return false;
        }

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}