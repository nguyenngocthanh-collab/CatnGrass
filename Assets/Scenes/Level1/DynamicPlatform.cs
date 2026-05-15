using UnityEngine;
using System.Collections;

public class DynamicPlatform : MonoBehaviour
{
    [Header("===== BẬT / TẮT =====")]
    public bool enableMovement = true;

    [Header("Random Behavior")]
    public bool randomBehavior = false;

    [Header("Random Hide Distance")]
    public bool randomHideOffset = false;

    [Header("===== DELAY BAN ĐẦU =====")]
    public float startDelay = 5f;

    [Header("===== HƯỚNG DI CHUYỂN =====")]
    public bool moveLeft = true;

    [Header("Khoảng lùi cố định")]
    public float hideDistance = 5f;

    [Header("Random khoảng lùi")]
    public float hideOffsetMin = 2f;
    public float hideOffsetMax = 8f;

    [Header("===== TỐC ĐỘ =====")]
    public float moveSpeed = 3f;

    [Header("Random Speed")]
    public float minSpeed = 2f;
    public float maxSpeed = 6f;

    [Header("===== THỜI GIAN =====")]
    public float visibleTime = 2f;

    public float hiddenTime = 2f;

    [Header("Random Wait")]
    public float minWait = 1f;
    public float maxWait = 4f;

    [Header("===== COLLIDER =====")]
    public bool disableColliderWhenHidden = false;

    private Collider2D col;

    private Vector3 showPosition;
    private Vector3 hidePosition;

    void Start()
    {
        col = GetComponent<Collider2D>();

        // TỰ LẤY vị trí hiện tại
        showPosition = transform.position;

        CalculateHidePosition();

        if (enableMovement)
        {
            StartCoroutine(PlatformLoop());
        }
    }

    void CalculateHidePosition()
    {
        float distance = hideDistance;

        if (randomHideOffset)
        {
            distance = Random.Range(hideOffsetMin, hideOffsetMax);
        }

        // Tính offset theo vị trí hiện tại
        if (moveLeft)
        {
            hidePosition = showPosition + Vector3.left * distance;
        }
        else
        {
            hidePosition = showPosition + Vector3.right * distance;
        }
    }

    IEnumerator PlatformLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            float currentVisibleTime = visibleTime;
            float currentHiddenTime = hiddenTime;
            float currentSpeed = moveSpeed;

            if (randomBehavior)
            {
                currentVisibleTime = Random.Range(minWait, maxWait);
                currentHiddenTime = Random.Range(minWait, maxWait);
                currentSpeed = Random.Range(minSpeed, maxSpeed);
            }

            CalculateHidePosition();

            // CHỜ
            yield return new WaitForSeconds(currentVisibleTime);

            // Tắt collider
            if (disableColliderWhenHidden && col != null)
            {
                col.enabled = false;
            }

            // ĐI RA
            yield return StartCoroutine(MoveTo(hidePosition, currentSpeed));

            // CHỜ
            yield return new WaitForSeconds(currentHiddenTime);

            // Bật collider
            if (disableColliderWhenHidden && col != null)
            {
                col.enabled = true;
            }

            // ĐI VÀO
            yield return StartCoroutine(MoveTo(showPosition, currentSpeed));
        }
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
    }
    // ===== DEV FUNCTIONS =====

    public void SetMovementState(bool state)
    {
        enableMovement = state;

        StopAllCoroutines();

        // Reset về vị trí gốc
        transform.position = showPosition;

        // Bật lại collider
        if (col != null)
        {
            col.enabled = true;
        }

        // Nếu bật lại thì chạy tiếp
        if (enableMovement)
        {
            StartCoroutine(PlatformLoop());
        }
    }

    public void ResetPlatform()
    {
        StopAllCoroutines();

        transform.position = showPosition;

        if (col != null)
        {
            col.enabled = true;
        }

        if (enableMovement)
        {
            StartCoroutine(PlatformLoop());
        }
    }
}