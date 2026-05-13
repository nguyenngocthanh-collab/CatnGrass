using UnityEngine;

public class Level3FlappyScreenClamp : MonoBehaviour
{
    [Header("Padding")]
    [SerializeField] private float topPadding = 0.5f;

    [SerializeField] private float bottomPadding = 0.5f;

    [Header("Death")]
    [SerializeField] private bool dieWhenHitBounds = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLines = false;

    private Camera mainCamera;

    private float minY;
    private float maxY;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        UpdateBounds();

        Vector3 position = transform.position;

        // Clamp player trong màn hình
        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY
        );

        transform.position = position;

        // Chết khi chạm giới hạn
        if (dieWhenHitBounds)
        {
            if (
                position.y <= minY + 0.01f ||
                position.y >= maxY - 0.01f
            )
            {
                if (
                    Level3FlappyGameManager.Instance != null
                )
                {
                    Level3FlappyGameManager
                        .Instance
                        .PlayerDied();
                }

                enabled = false;
            }
        }
    }

    private void UpdateBounds()
    {
        minY =
            mainCamera.ViewportToWorldPoint(
                new Vector3(0, 0, 0)
            ).y + bottomPadding;

        maxY =
            mainCamera.ViewportToWorldPoint(
                new Vector3(0, 1, 0)
            ).y - topPadding;
    }

    // DEBUG VISUAL
    private void OnDrawGizmos()
    {
        if (!showDebugLines) return;

        if (Camera.main == null) return;

        Camera cam = Camera.main;

        float left =
            cam.ViewportToWorldPoint(
                new Vector3(0, 0, 0)
            ).x;

        float right =
            cam.ViewportToWorldPoint(
                new Vector3(1, 0, 0)
            ).x;

        float debugMinY =
            cam.ViewportToWorldPoint(
                new Vector3(0, 0, 0)
            ).y + bottomPadding;

        float debugMaxY =
            cam.ViewportToWorldPoint(
                new Vector3(0, 1, 0)
            ).y - topPadding;

        Gizmos.color = Color.red;

        // TOP LINE
        Gizmos.DrawLine(
            new Vector3(left, debugMaxY, 0),
            new Vector3(right, debugMaxY, 0)
        );

        // BOTTOM LINE
        Gizmos.DrawLine(
            new Vector3(left, debugMinY, 0),
            new Vector3(right, debugMinY, 0)
        );
    }
}