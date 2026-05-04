using UnityEngine;

public class CameraFollowv2 : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 10f;
    public Vector3 offset = new Vector3(0, 1, -10);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            0f
        ) + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }
}