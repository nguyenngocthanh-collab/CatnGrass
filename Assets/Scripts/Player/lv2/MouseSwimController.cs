using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private float maxSpeed = 10f;

    private bool facingRight = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        FollowMousePositionDelayed(maxSpeed);

        FlipTowardsMouse();
    }

    private void FollowMousePositionDelayed(float maxSpeed)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            GetWorldPositionFromMouse(),
            maxSpeed * Time.deltaTime
        );
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 10f;

        Vector3 worldPos =
            mainCamera.ScreenToWorldPoint(mousePos);

        worldPos.z = 0f;

        return worldPos;
    }

    private void FlipTowardsMouse()
    {
        Vector3 mouseWorldPos = GetWorldPositionFromMouse();

        // Chuột bên phải
        if (mouseWorldPos.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        // Chuột bên trái
        else if (mouseWorldPos.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 localScale = transform.localScale;

        localScale.x *= -1f;

        transform.localScale = localScale;
    }
}