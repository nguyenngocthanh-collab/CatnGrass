using UnityEngine;

/// <summary>
/// FLOW:
///   1. Spawn xa ngoài màn hình, đứng IM trong holdTime giây
///   2. Warning nhấp nháy ở mép camera (KHÔNG xoay sprite)
///   3. Sau holdTime → phóng, warning mất khi meteor vào camera
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Meteor : MonoBehaviour
{
    // ── Warning ──────────────────────────────────────────
    [Header("Warning")]

    [Tooltip("Prefab chỉ cần có SpriteRenderer")]
    [SerializeField] private GameObject warningIndicatorPrefab;

    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0f, 1f);

    [Tooltip("Scale của warning, độc lập với meteor")]
    [SerializeField] private float warningScale = 1f;

    [Tooltip("Khoảng lùi từ mép màn hình (viewport 0-1). 0 = sát mép")]
    [SerializeField][Range(0f, 0.3f)] private float warningEdgePadding = 0.06f;

    [Tooltip("Giây mỗi chu kỳ bật/tắt. Nhỏ hơn = nhanh hơn")]
    [SerializeField][Range(0.05f, 0.8f)] private float warningBlinkInterval = 0.2f;

    // ── Hold Before Launch ────────────────────────────────
    [Header("Hold Before Launch")]

    [Tooltip("Giây đứng im trước khi phóng — cũng là thời gian warning hiển thị")]
    [SerializeField] private float holdTime = 1.5f;

    [Tooltip("Ẩn sprite meteor trong lúc hold (chỉ hiện warning)")]
    [SerializeField] private bool hideWhileHolding = true;

    // ── Lifetime ──────────────────────────────────────────
    [Header("Lifetime")]

    [Tooltip("Tính từ lúc phóng")]
    [SerializeField] private float maxLifetime = 15f;

    // ── Internal ──────────────────────────────────────────
    private Vector2 _velocity;
    private float _minSize, _maxSize;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Camera _cam;

    private bool _launched;
    private bool _entered;
    private float _holdElapsed;
    private GameObject _warningInstance;

    // ── Init (gọi trước Start) ────────────────────────────
    public void InitMeteor(Vector2 velocity, float minSize, float maxSize)
    {
        _velocity = velocity;
        _minSize = minSize;
        _maxSize = maxSize;
    }

    // ── Awake ─────────────────────────────────────────────
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _cam = Camera.main;

        _rb.gravityScale = 0f;
        _rb.freezeRotation = false;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }

    // ── Start ─────────────────────────────────────────────
    private void Start()
    {
        // Random size
        float size = Random.Range(_minSize, _maxSize);
        transform.localScale = Vector3.one * size;

        // Xoay meteor theo hướng bay
        float angle =
            Mathf.Atan2(_velocity.y, _velocity.x) *
            Mathf.Rad2Deg - 90f;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        if (hideWhileHolding)
            _sr.enabled = false;

        if (warningIndicatorPrefab != null)
            SpawnWarning();

        Destroy(gameObject, holdTime + maxLifetime);
    }

    // ── Update ────────────────────────────────────────────
    private void Update()
    {
        if (!_launched)
        {
            _holdElapsed += Time.deltaTime;

            if (_holdElapsed >= holdTime)
                Launch();

            return;
        }

        if (!_entered)
        {
            if (IsInsideViewport())
            {
                _entered = true;

                DestroyWarning();
            }
        }
        else
        {
            if (!IsInsideViewport(2f))
            {
                Destroy(gameObject);
            }
        }
    }

    // ── Launch ────────────────────────────────────────────
    private void Launch()
    {
        _launched = true;

        _sr.enabled = true;

        _rb.linearVelocity = _velocity;

        _rb.angularVelocity =
            Random.Range(-180f, 180f);
    }

    // ── Warning ───────────────────────────────────────────
    private void SpawnWarning()
    {
        _warningInstance = Instantiate(
            warningIndicatorPrefab,
            Vector3.zero,
            Quaternion.identity);

        _warningInstance.transform.localScale =
            Vector3.one * warningScale;

        var wsr =
            _warningInstance.GetComponent<SpriteRenderer>();

        if (wsr != null)
        {
            wsr.color = warningColor;
        }

        var blink =
            _warningInstance.AddComponent<WarningBlinker>();

        blink.Init(warningBlinkInterval);

        var tracker =
            _warningInstance.AddComponent<EdgeTracker>();

        tracker.target = transform;
        tracker.padding = warningEdgePadding;
    }

    private void DestroyWarning()
    {
        if (_warningInstance != null)
        {
            Destroy(_warningInstance);

            _warningInstance = null;
        }
    }

    // ── Viewport ──────────────────────────────────────────
    private bool IsInsideViewport(float margin = 0f)
    {
        if (_cam == null)
            return true;

        Vector3 vp =
            _cam.WorldToViewportPoint(transform.position);

        return vp.x > -margin &&
               vp.x < 1f + margin &&
               vp.y > -margin &&
               vp.y < 1f + margin;
    }

    // ── DEBUG ─────────────────────────────────────────────
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(
            "METEOR HIT: " +
            collision.collider.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            "METEOR TRIGGER: " +
            other.name);
    }

    // ── DESTROY ───────────────────────────────────────────
    private void OnDestroy()
    {
        DestroyWarning();
    }
}

// =========================================================
// WarningBlinker
// =========================================================
public class WarningBlinker : MonoBehaviour
{
    private float _interval = 0.2f;
    private float _elapsed;
    private SpriteRenderer _sr;

    public void Init(float blinkInterval)
    {
        _interval = Mathf.Max(0.05f, blinkInterval);

        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;

        if (_sr != null)
        {
            _sr.enabled =
                (int)(_elapsed / _interval) % 2 == 0;
        }
    }
}

// =========================================================
// EdgeTracker
// =========================================================
public class EdgeTracker : MonoBehaviour
{
    public Transform target;

    [Range(0f, 0.3f)]
    public float padding = 0.06f;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);

            return;
        }

        if (_cam == null)
            return;

        Vector3 vp =
            _cam.WorldToViewportPoint(target.position);

        bool inView =
            vp.x >= 0f &&
            vp.x <= 1f &&
            vp.y >= 0f &&
            vp.y <= 1f &&
            vp.z > 0f;

        if (inView)
        {
            gameObject.SetActive(false);

            return;
        }

        gameObject.SetActive(true);

        vp.x =
            Mathf.Clamp(vp.x, padding, 1f - padding);

        vp.y =
            Mathf.Clamp(vp.y, padding, 1f - padding);

        float depth =
            Mathf.Abs(_cam.transform.position.z);

        Vector3 world =
            _cam.ViewportToWorldPoint(
                new Vector3(vp.x, vp.y, depth));

        world.z = 0f;

        transform.position = world;
    }
}