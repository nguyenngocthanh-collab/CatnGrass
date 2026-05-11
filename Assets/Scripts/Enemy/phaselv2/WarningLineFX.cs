using System.Collections;
using UnityEngine;

// ============================================================
//  WarningLineFX.cs
// ============================================================
public class WarningLineFX : MonoBehaviour
{
    [Header("── References ──")]
    public SpriteRenderer lineBody;

    [Header("── Size ──")]
    public float lineLength = 12f;
    public float lineThickness = 0.5f;

    [Header("── Position Offset ──")]
    [Tooltip("+ = đẩy ra xa miệng cá, - = kéo vào trong")]
    public float forwardOffset = 0f;
    public float lateralOffset = 0f;

    [Header("── Pulse ──")]
    [Range(0f, 1f)] public float minAlpha = 0.15f;
    [Range(0f, 1f)] public float maxAlpha = 0.9f;
    public float pulseSpeed = 3f;

    [Header("── Fade Out ──")]
    public float fadeOutDuration = 0.35f;

    bool _fading;

    // =====================================================

    public void Init(Vector2 direction, float length = -1f, float thickness = -1f)
    {
        if (length > 0f) lineLength = length;
        if (thickness > 0f) lineThickness = thickness;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        ApplyVisual();
        StartCoroutine(PulseLoop());
    }

    // =====================================================

    void ApplyVisual()
    {
        if (lineBody == null || lineBody.sprite == null) return;

        // Set scale trực tiếp = world units, không tính native size
        Vector3 s = lineBody.transform.localScale;
        s.x = lineLength;
        s.y = lineThickness;
        lineBody.transform.localScale = s;

        // Pivot sprite để ở CENTER (default) → offset nửa length về trước
        lineBody.transform.localPosition = new Vector3(
            lineLength * 0.5f + forwardOffset,
            lateralOffset,
            0f
        );
    }

    // =====================================================

    IEnumerator PulseLoop()
    {
        float t = 0f;
        while (!_fading)
        {
            t += Time.deltaTime * pulseSpeed;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(t) + 1f) * 0.5f);
            SetAlpha(alpha);
            yield return null;
        }
    }

    // =====================================================

    public void FadeOutAndDestroy()
    {
        if (_fading) return;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        _fading = true;
        float startAlpha = GetCurrentAlpha();
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(fadeOutDuration, 0.01f);
            SetAlpha(Mathf.Lerp(startAlpha, 0f, t));
            yield return null;
        }

        Destroy(gameObject);
    }

    // =====================================================

    void SetAlpha(float a)
    {
        if (lineBody == null) return;
        Color c = lineBody.color;
        c.a = a;
        lineBody.color = c;
    }

    float GetCurrentAlpha() =>
        lineBody != null ? lineBody.color.a : maxAlpha;
}