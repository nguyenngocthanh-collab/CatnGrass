using UnityEngine;

/// <summary>
/// G?n vào prefab thiên th?ch.
/// - Random kích th??c (scale + collider)
/// - Bay theo h??ng ???c set t? ngoài (MeteorSpawner g?i Init)
/// - T? h?y khi ra ngoài viewport + lifetime
/// - T?o Warning Indicator trên rìa màn hình tr??c khi vào
/// 
/// REQUIRES: DamageDealer ?ã g?n s?n trên prefab (ho?c child)
/// REQUIRES: Prefab có SpriteRenderer + CircleCollider2D (ho?c PolygonCollider2D)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Meteor : MonoBehaviour
{
    [Header("Visual Warning")]
    [Tooltip("Prefab sprite m?i tên / ch?m c?nh báo hi?n trên rìa màn hình")]
    [SerializeField] private GameObject warningIndicatorPrefab;

    [Tooltip("Màu warning indicator")]
    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0f, 1f);

    [Tooltip("Giây hi?n th? warning tr??c khi meteor vào màn hình (0 = không warning)")]
    [SerializeField] private float warningDuration = 1.2f;

    [Header("Lifetime")]
    [Tooltip("T? h?y sau bao giây k? t? khi spawn (phòng tr??ng h?p bay ra ngoài mà không trigger exit)")]
    [SerializeField] private float maxLifetime = 15f;

    // ?? ???c set b?i MeteorSpawner.InitMeteor() ??????????????????????????????
    private Vector2 _velocity;
    private float _minSize;
    private float _maxSize;

    // internal
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private GameObject _warningInstance;
    private bool _entered = false; // ?ã vào viewport ch?a
    private Camera _cam;

    // ?? Public init — MeteorSpawner g?i sau Instantiate ??????????????????????
    /// <param name="velocity">H??ng + t?c ?? (world units/s)</param>
    /// <param name="minSize">Scale nh? nh?t</param>
    /// <param name="maxSize">Scale l?n nh?t</param>
    public void InitMeteor(Vector2 velocity, float minSize, float maxSize)
    {
        _velocity = velocity;
        _minSize = minSize;
        _maxSize = maxSize;
    }

    // ?????????????????????????????????????????????????????????????????????????
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _cam = Camera.main;

        _rb.gravityScale = 0f;
        _rb.freezeRotation = false;
    }

    private void Start()
    {
        // Random kích th??c
        float size = Random.Range(_minSize, _maxSize);
        transform.localScale = Vector3.one * size;

        // Xoay sprite theo h??ng bay
        float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set velocity
        _rb.linearVelocity = _velocity;

        // T? xoay nh? khi bay (aesthetic)
        _rb.angularVelocity = Random.Range(-180f, 180f);

        // H?y sau lifetime t?i ?a
        Destroy(gameObject, maxLifetime);

        // Hi?n warning n?u c?n
        if (warningDuration > 0f && warningIndicatorPrefab != null)
            SpawnWarning();
    }

    private void Update()
    {
        // Ki?m tra ?ã vào viewport ch?a ?? b?t ??u track exit
        if (!_entered)
        {
            if (IsInsideViewport())
            {
                _entered = true;
                // Xóa warning khi ?ã vào màn
                if (_warningInstance != null)
                    Destroy(_warningInstance);
            }
        }
        else
        {
            // ?ã vào r?i mà ra ngoài ? h?y
            if (!IsInsideViewport(margin: 2f))
                Destroy(gameObject);
        }
    }

    // ?? Warning Indicator ????????????????????????????????????????????????????
    private void SpawnWarning()
    {
        // Tính v? trí rìa màn hình g?n nh?t theo h??ng ng??c l?i
        Vector3 edgePos = GetEdgePosition();
        _warningInstance = Instantiate(warningIndicatorPrefab, edgePos, Quaternion.identity);

        // Xoay indicator tr? v? h??ng bay ??n
        float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
        _warningInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Tô màu warning
        var wsr = _warningInstance.GetComponent<SpriteRenderer>();
        if (wsr != null) wsr.color = warningColor;

        // Nh?p nháy r?i t? h?y
        var blink = _warningInstance.AddComponent<WarningBlinker>();
        blink.Init(warningDuration);

        // G?n theo rìa màn hình (update position)
        var tracker = _warningInstance.AddComponent<EdgeTracker>();
        tracker.Init(transform, _cam);
    }

    private Vector3 GetEdgePosition()
    {
        if (_cam == null) return transform.position;

        float h = _cam.orthographicSize;
        float w = h * _cam.aspect;
        Vector3 camPos = _cam.transform.position;

        // H??ng t? meteor ??n màn hình
        Vector2 dir = _velocity.normalized;

        // Intersect v?i 4 c?nh viewport
        // ??n gi?n: clamp vào viewport bounds
        Vector3 pos = transform.position;
        float tx = dir.x != 0 ? (dir.x > 0 ? (camPos.x + w - pos.x) / dir.x : (camPos.x - w - pos.x) / dir.x) : float.MaxValue;
        float ty = dir.y != 0 ? (dir.y > 0 ? (camPos.y + h - pos.y) / dir.y : (camPos.y - h - pos.y) / dir.y) : float.MaxValue;
        float t = Mathf.Min(Mathf.Abs(tx), Mathf.Abs(ty));
        if (t < 0) t = 0;

        Vector3 edge = pos + new Vector3(dir.x, dir.y, 0) * t;
        edge.x = Mathf.Clamp(edge.x, camPos.x - w, camPos.x + w);
        edge.y = Mathf.Clamp(edge.y, camPos.y - h, camPos.y + h);
        edge.z = 0;
        return edge;
    }

    private bool IsInsideViewport(float margin = 0f)
    {
        if (_cam == null) return true;
        Vector3 vp = _cam.WorldToViewportPoint(transform.position);
        return vp.x > -margin && vp.x < 1 + margin &&
               vp.y > -margin && vp.y < 1 + margin;
    }

    private void OnDestroy()
    {
        if (_warningInstance != null)
            Destroy(_warningInstance);
    }
}

// ?? Helper Components (nh?, t? xóa) ?????????????????????????????????????????

/// <summary>Nh?p nháy SpriteRenderer r?i t? h?y GameObject</summary>
public class WarningBlinker : MonoBehaviour
{
    private float _lifetime;
    private float _elapsed;
    private SpriteRenderer _sr;
    private float _blinkRate = 0.15f;

    public void Init(float lifetime)
    {
        _lifetime = lifetime;
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        if (_sr != null)
        {
            bool visible = (int)(_elapsed / _blinkRate) % 2 == 0;
            _sr.enabled = visible;
        }
        if (_elapsed >= _lifetime)
            Destroy(gameObject);
    }
}

/// <summary>Gi? warning indicator luôn ? rìa màn hình theo h??ng meteor</summary>
public class EdgeTracker : MonoBehaviour
{
    private Transform _target;
    private Camera _cam;

    public void Init(Transform target, Camera cam)
    {
        _target = target;
        _cam = cam;
    }

    private void LateUpdate()
    {
        if (_target == null || _cam == null) return;

        float h = _cam.orthographicSize;
        float w = h * _cam.aspect;
        Vector3 camPos = _cam.transform.position;
        Vector3 dir = (_target.position - _cam.transform.position).normalized;

        float tx = dir.x != 0 ? (dir.x > 0 ? (camPos.x + w) : (camPos.x - w)) : float.MaxValue;
        float ty = dir.y != 0 ? (dir.y > 0 ? (camPos.y + h) : (camPos.y - h)) : float.MaxValue;

        float ratioX = dir.x != 0 ? Mathf.Abs((tx - camPos.x) / dir.x) : float.MaxValue;
        float ratioY = dir.y != 0 ? Mathf.Abs((ty - camPos.y) / dir.y) : float.MaxValue;

        Vector3 edge;
        if (ratioX < ratioY)
            edge = new Vector3(tx, camPos.y + dir.y * ratioX, 0);
        else
            edge = new Vector3(camPos.x + dir.x * ratioY, ty, 0);

        transform.position = edge;

        // Xoay indicator tr? v? meteor
        Vector2 toTarget = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}