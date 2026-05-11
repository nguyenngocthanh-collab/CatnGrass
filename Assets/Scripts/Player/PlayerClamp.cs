using UnityEngine;

public class PlayerClamp : MonoBehaviour
{
    public Transform boundsCenter;

    public Vector2 boundsSize = new Vector2(10f, 5f);

    void LateUpdate()
    {
        if (boundsCenter == null) return;

        Vector3 center = boundsCenter.position;

        float minX = center.x - boundsSize.x * 0.5f;
        float maxX = center.x + boundsSize.x * 0.5f;

        float minY = center.y - boundsSize.y * 0.5f;
        float maxY = center.y + boundsSize.y * 0.5f;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    void OnDrawGizmos()
    {
        if (boundsCenter == null) return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            boundsCenter.position,
            boundsSize
        );
    }
}