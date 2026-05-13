using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Level3FlappyBirdController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float flapForce = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float upRotation = 30f;
    [SerializeField] private float downRotation = -90f;

    private Rigidbody2D rb;

    private bool firstFlap;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Debug.Log("Bird Awake");
    }

    private void Start()
    {
        rb.gravityScale = 0f;

        Debug.Log("Bird Start");
    }

    private void Update()
    {
        if (Level3FlappyGameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance NULL");
            return;
        }

        if (Level3FlappyGameManager.Instance.IsDead)
            return;

        bool flapInput =
            Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space);

        if (flapInput)
        {
            Debug.Log("FLAP INPUT");

            HandleFlap();
        }

        RotateBird();
    }

    private void HandleFlap()
    {
        if (!firstFlap)
        {
            firstFlap = true;

            rb.gravityScale = gravityScale;

            Level3FlappyGameManager.Instance.StartGame();

            Debug.Log("GAME STARTED");
        }

        rb.linearVelocity = Vector2.up * flapForce;

        Debug.Log("BIRD FLAP");
    }

    private void RotateBird()
    {
        float velocityY = rb.linearVelocity.y;

        float normalizedVelocity =
            Mathf.InverseLerp(-5f, 5f, velocityY);

        float targetRotation =
            Mathf.Lerp(downRotation, upRotation, normalizedVelocity);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION WITH: " + collision.gameObject.name);

        if (Level3FlappyGameManager.Instance == null)
        {
            Debug.LogError("GameManager NULL");
            return;
        }

        if (Level3FlappyGameManager.Instance.IsDead)
        {
            Debug.Log("Already Dead");
            return;
        }

        Debug.Log("TAG: " + collision.gameObject.tag);

        if (
            collision.gameObject.CompareTag("Pipe") ||
            collision.gameObject.CompareTag("Ground")
        )
        {
            Debug.Log("DEATH TRIGGERED");

            Die();
        }
        else
        {
            Debug.Log("Wrong Tag");
        }
    }

    private void Die()
    {
        Debug.Log("DIE FUNCTION");

        enabled = false;

        rb.linearVelocity = Vector2.zero;

        Level3FlappyGameManager.Instance.PlayerDied();
    }
}