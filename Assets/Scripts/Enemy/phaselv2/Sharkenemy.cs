using System.Collections;
using UnityEngine;

// ============================================================
//  SharkEnemy.cs
// ============================================================
[RequireComponent(typeof(Rigidbody2D))]
public class SharkEnemy : MonoBehaviour
{
    [Header("Warning FX Prefab")]
    public GameObject warningLinePrefab;

    [Header("Warning Override (0 = dùng giá trị prefab)")]
    public float warningLineLength = 0f;
    public float warningLineThickness = 0f;

    // =====================================================

    Rigidbody2D _rb;
    Camera _cam;
    float _camH, _camW;
    Vector2 _dir;
    float _speed;
    bool _showWarning;
    float _holdDuration;
    float _fadeOutDuration;
    WarningLineFX _warningFX;

    // =====================================================

    public void Init(
        Vector2 direction,
        float speed,
        bool showWarning,
        float holdDuration,
        float fadeOutDuration = 0.35f
    )
    {
        _dir = direction.normalized;
        _speed = speed;
        _showWarning = showWarning;
        _holdDuration = holdDuration;
        _fadeOutDuration = fadeOutDuration;

        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.isKinematic = true;

        _cam = Camera.main;
        _camH = _cam.orthographicSize;
        _camW = _camH * _cam.aspect;

        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        StartCoroutine(LifeCycle());
    }

    // =====================================================

    IEnumerator LifeCycle()
    {
        // 1. Đứng im + hiện warning
        if (_showWarning && _holdDuration > 0f)
        {
            SpawnWarningLine();
            yield return new WaitForSeconds(_holdDuration);
        }

        // 2. Fade warning + bắt đầu lao đồng thời
        _warningFX?.FadeOutAndDestroy();

        _rb.isKinematic = false;
        _rb.linearVelocity = _dir * _speed;

        // 3. Chờ ra ngoài viewport
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(IsOutsideView);

        Destroy(gameObject);
    }

    // =====================================================

    void SpawnWarningLine()
    {
        if (warningLinePrefab == null) return;

        GameObject go = Instantiate(
            warningLinePrefab,
            transform.position,
            Quaternion.identity,
            null
        );

        _warningFX = go.GetComponent<WarningLineFX>();
        _warningFX?.Init(
            _dir,
            warningLineLength > 0f ? warningLineLength : -1f,
            warningLineThickness > 0f ? warningLineThickness : -1f
        );
    }

    // =====================================================

    bool IsOutsideView()
    {
        Vector3 camPos = _cam.transform.position;
        const float margin = 2f;
        Vector3 pos = transform.position;

        return
            pos.x < camPos.x - _camW - margin ||
            pos.x > camPos.x + _camW + margin ||
            pos.y < camPos.y - _camH - margin ||
            pos.y > camPos.y + _camH + margin;
    }

    void OnDestroy()
    {
        _warningFX?.FadeOutAndDestroy();
    }
}