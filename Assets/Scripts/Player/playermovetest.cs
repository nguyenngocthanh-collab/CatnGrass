using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMoveTest : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private Transform tf;

    [Header("Layer")]
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveForce = 20f;
    public float maxMoveSpeed = 8f;
    public float dampening = 0.7f;
    public float threshold = 0.1f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpCooldownTime = 0.2f;
    public float coyoteTime = 0.1f;
    public int jumpMemoryFrames = 5;

    [Header("Extra")]
    public float extraGravity = 20f;
    public float crouchForce = 5f;
    public float angleLimit = 30f;

    // Internal state
    private Vector2 direction;
    private bool jumpEnabled = true;
    private bool coyoteOn = false;
    private bool jumpCooldown = true;
    private int jumpPressed;
    private bool crouch;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tf = transform;
    }

    void Update()
    {
        HandleInput();

        // Debug reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            tf.position = Vector3.up * 3f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = jumpMemoryFrames;
        }

        crouch = Input.GetKey(KeyCode.S);
    }

    private void HandleMovement()
    {
        // Counter movement
        if (Mathf.Abs(direction.x) < threshold)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * dampening, rb.linearVelocity.y);
        }

        // Extra gravity
        rb.AddForce(Vector2.down * extraGravity);

        // Jump buffer
        if (jumpPressed > 0)
        {
            jumpPressed--;

            if ((jumpEnabled || coyoteOn) && jumpCooldown)
            {
                jumpPressed = 0;
                jumpEnabled = false;
                jumpCooldown = false;
                coyoteOn = false;

                Invoke(nameof(ResetJumpCooldown), jumpCooldownTime);
                Jump();
            }
        }

        // Horizontal movement
        float targetVelocityX = rb.linearVelocity.x + direction.x * moveForce;

        if (targetVelocityX > maxMoveSpeed)
        {
            rb.linearVelocity = new Vector2(maxMoveSpeed, rb.linearVelocity.y);
        }
        else if (targetVelocityX < -maxMoveSpeed)
        {
            rb.linearVelocity = new Vector2(-maxMoveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.AddForce(new Vector2(direction.x * moveForce, 0));
        }

        // Crouch
        if (crouch)
        {
            rb.AddForce(Vector2.down * crouchForce, ForceMode2D.Impulse);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGround(collision.gameObject.layer))
        {
            foreach (var contact in collision.contacts)
            {
                float angle = Vector2.Angle(Vector2.up, contact.normal);

                if (angle <= angleLimit)
                {
                    jumpEnabled = true;
                    coyoteOn = true;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGround(collision.gameObject.layer))
        {
            jumpEnabled = false;
            Invoke(nameof(DisableCoyote), coyoteTime);
        }
    }

    private bool IsGround(int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0;
    }

    private void ResetJumpCooldown()
    {
        jumpCooldown = true;
    }

    private void DisableCoyote()
    {
        coyoteOn = false;
    }
}