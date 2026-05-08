using UnityEngine;

public class CameraFollowv2 : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    [Range(1f, 30f)]
    public float followSpeed = 10f;

    [Header("Camera Offset")]
    [Tooltip("Lệch trái/phải")]
    [Range(-20f, 20f)]
    public float offsetX = 0f;

    [Tooltip("Lệch lên/xuống")]
    [Range(-20f, 20f)]
    public float offsetY = 2f;

    [Tooltip("Khoảng cách camera")]
    [Range(-50f, -1f)]
    public float offsetZ = -10f;

    [Header("Camera Limits")]
    public bool useLimit = false;

    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    [Header("Look Ahead")]
    public bool lookAhead = false;

    [Range(0f, 10f)]
    public float lookAheadAmount = 2f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        float finalX = target.position.x + offsetX;
        float finalY = target.position.y + offsetY;

        // Camera nhìn trước hướng di chuyển
        if (lookAhead)
        {
            float moveX = Input.GetAxisRaw("Horizontal");

            finalX += moveX * lookAheadAmount;
        }

        // Giới hạn camera
        if (useLimit)
        {
            finalX = Mathf.Clamp(finalX, minX, maxX);
            finalY = Mathf.Clamp(finalY, minY, maxY);
        }

        Vector3 targetPos = new Vector3(
            finalX,
            finalY,
            offsetZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    // Vẽ vùng giới hạn trong Scene
    void OnDrawGizmosSelected()
    {
        if (!useLimit) return;

        Gizmos.color = Color.green;

        Vector3 center = new Vector3(
            (minX + maxX) / 2,
            (minY + maxY) / 2,
            0
        );

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0
        );

        Gizmos.DrawWireCube(center, size);
    }
}