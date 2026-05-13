using UnityEngine;

public class Level2EndingPortal : MonoBehaviour
{
    [Header("PORTAL TIMER")]
    [SerializeField] private float portalSpawnTime = 55f;

    private float timer;

    [Header("MOVE")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("PULL")]
    [SerializeField] private float pullSpeed = 15f;

    [SerializeField] private float pullDistance = 10f;

    [Header("PLAYER")]
    [SerializeField] private Transform player;

    private bool activePortal;

    private bool pulling;

    void Start()
    {
        gameObject.SetActive(false);

        timer = portalSpawnTime;
    }

    void Update()
    {
        // timer riêng của portal
        if (!activePortal)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                activePortal = true;

                gameObject.SetActive(true);
            }

            return;
        }

        // portal di chuyển
        transform.position +=
            -transform.right *
            moveSpeed *
            Time.deltaTime;

        if (player == null)
            return;

        float dist =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // bắt đầu hút
        if (!pulling &&
            dist <= pullDistance)
        {
            pulling = true;

            Rigidbody rb =
                player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;

                rb.useGravity = false;
            }

            Collider col =
                player.GetComponent<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }
        }

        // hút player
        if (pulling)
        {
            player.position =
                Vector3.MoveTowards(
                    player.position,
                    transform.position,
                    pullSpeed * Time.deltaTime
                );
        }
    }
}