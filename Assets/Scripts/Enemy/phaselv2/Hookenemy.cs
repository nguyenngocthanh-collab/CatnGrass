using System.Collections;
using UnityEngine;


// ============================================================
//  HookEnemy.cs
// ============================================================
[RequireComponent(typeof(Rigidbody2D))]
public class HookEnemy : MonoBehaviour
{
    Rigidbody2D _rb;
    Camera _cam;
    void Start()
    {
        Debug.Log("HOOK START POS = " + transform.position);
    }

    // Runtime values
    float _castSpeed;
    float _retractSpeed;
    float _castDistance;
    float _holdAtBottom;

    bool _showWarningLine;
    float _holdBeforeCast;
    float _warningFadeOut;

    // ========================================================

    public void Init(
        float castSpeed,
        float retractSpeed,
        float castDistance,
        float holdAtBottom,
        bool showWarningLine,
        float holdBeforeCast,
        float warningFadeOut
    )
    {
        _castSpeed = castSpeed;
        _retractSpeed = retractSpeed;
        _castDistance = castDistance;
        _holdAtBottom = holdAtBottom;

        _showWarningLine = showWarningLine;
        _holdBeforeCast = holdBeforeCast;
        _warningFadeOut = warningFadeOut;

        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.isKinematic = true;

        _cam = Camera.main;

        StartCoroutine(HookCycle());
    }

    // ========================================================

    IEnumerator HookCycle()
    {
        Vector3 startPos = transform.position;

        // Hook luôn đâm xuống
        Vector3 moveDir = Vector3.down;

        // ====================================================
        // WARNING HOLD
        // ====================================================

        if (_holdBeforeCast > 0f)
            yield return new WaitForSeconds(_holdBeforeCast);

        // ====================================================
        // CAST DOWN
        // ====================================================

        yield return MoveDistance(
            moveDir,
            _castDistance,
            _castSpeed
        );

        // ====================================================
        // HOLD BOTTOM
        // ====================================================

        if (_holdAtBottom > 0f)
            yield return new WaitForSeconds(_holdAtBottom);

        // ====================================================
        // RETRACT
        // ====================================================

        yield return MoveDistance(
            -moveDir,
            _castDistance,
            _retractSpeed
        );

        Destroy(gameObject);
    }

    // ========================================================

    IEnumerator MoveDistance(
        Vector3 dir,
        float distance,
        float speed
    )
    {
        Vector3 start = transform.position;
        Vector3 target = start + dir.normalized * distance;

        while (Vector3.Distance(transform.position, target) > 0.02f)
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
}