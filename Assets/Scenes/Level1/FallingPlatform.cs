using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private float moveSpeed;

    private Camera cam;

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        transform.Translate(
            Vector3.down
            * moveSpeed
            * Time.deltaTime);

        if (cam == null) return;

        float destroyY =
            cam.transform.position.y
            - cam.orthographicSize
            - 10f;

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}