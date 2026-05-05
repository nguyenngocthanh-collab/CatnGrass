using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class playermovetest2 : MonoBehaviour
{
    private Rigidbody2D rb;

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

    private Vector2 direction;
    private Vector2 forceVector;

    private bool jumpEnabled = false;
    private bool coyoteOn = false;
    private bool jumpCooldown = true;
    private int jumpPressed;
    private bool crouch;

    private float jumpCooldownTimer = 0f;
    private float coyoteTimer = 0f;
    private bool coyoteCountingDown = false;

    private ContactPoint2D[] contacts = new ContactPoint2D[8];

    public bool IsGrounded => jumpEnabled;
    void Awake()
    {
         rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        TickTimers();

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.up * 3f;
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
            jumpPressed = jumpMemoryFrames;

        crouch = Input.GetKey(KeyCode.S);
    }

    private void TickTimers()
    {
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
            if (jumpCooldownTimer <= 0f)
                jumpCooldown = true;
        }

        if (coyoteCountingDown)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
            {
                coyoteOn = false;
                coyoteCountingDown = false;
            }
        }
    }

    private void HandleMovement()
    {
        // Counter movement
        if (Mathf.Abs(direction.x) < threshold)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * dampening, rb.linearVelocity.y);
        }

        // Extra gravity — Force mode, KHÔNG nhân deltaTime
        rb.AddForce(Vector2.down * extraGravity, ForceMode2D.Force);

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
                coyoteCountingDown = false;
                jumpCooldownTimer = jumpCooldownTime;
                Jump();
            }
        }

        // Horizontal movement
        float targetVelocityX = rb.linearVelocity.x + direction.x * moveForce * Time.fixedDeltaTime;

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
            forceVector.x = direction.x * moveForce;
            forceVector.y = 0f;
            rb.AddForce(forceVector);
        }

        if (crouch)
            rb.AddForce(Vector2.down * crouchForce, ForceMode2D.Impulse);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // OnCollisionEnter — phát hiện chạm đất
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsGround(collision.gameObject.layer)) return;
        CheckGroundContacts(collision);
    }

    // OnCollisionStay — giữ jumpEnabled khi đứng yên trên đất
    // Fix edge snag: Stay liên tục verify contact normal
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsGround(collision.gameObject.layer)) return;
        CheckGroundContacts(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsGround(collision.gameObject.layer)) return;

        jumpEnabled = false;
        coyoteTimer = coyoteTime;
        coyoteCountingDown = true;
    }

    // Tách ra hàm riêng để Enter và Stay dùng chung
    private void CheckGroundContacts(Collision2D collision)
    {
        int count = collision.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            if (Vector2.Angle(Vector2.up, contacts[i].normal) <= angleLimit)
            {
                jumpEnabled = true;
                coyoteOn = true;
                coyoteCountingDown = false;
                return;
            }
        }
    }

    private bool IsGround(int layer) => (groundLayer.value & (1 << layer)) != 0;
}