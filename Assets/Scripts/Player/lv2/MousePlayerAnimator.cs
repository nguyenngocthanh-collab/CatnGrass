using UnityEngine;

public class MousePlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualTransform;

    [SerializeField] private float flipThreshold = 0.1f;

    private Camera mainCamera;
    private bool facingRight = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        FlipTowardsMouse();
    }

    private void FlipTowardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        if (worldPos.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (worldPos.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = visualTransform.localScale;
        scale.x *= -1f;
        visualTransform.localScale = scale;
    }
}