using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 10f;
    public Vector3 offset = new Vector3(0, 1, -10);

    private Rigidbody2D targetRb;

    void Start()
    {
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition;

        // Nếu có Rigidbody → lấy vị trí chuẩn physics
        if (targetRb != null)
        {
            targetPosition = new Vector3(
                targetRb.position.x,
                targetRb.position.y,
                0f
            );
        }
        else
        {
            targetPosition = target.position;
        }

        targetPosition += offset;

        // Move mượt nhưng không lệch frame
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.fixedDeltaTime
        );
    }
}