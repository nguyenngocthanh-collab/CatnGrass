using UnityEngine;
using System.Collections;

/// <summary>
/// Meteor Spawner System
/// 
/// HỖ TRỢ:
/// - Spawn liên tục
/// - Spawn theo trigger
/// - Spawn theo player
/// - Perspective + Orthographic camera
/// - Random tốc độ
/// - Random góc
/// - Random scale
/// - Random wave
/// - Random cạnh màn hình
/// - Spawn quanh player
/// - Fixed zone
/// - Warning prefab
/// - Limit meteor
/// - Auto cleanup
/// 
/// METEOR PREFAB CẦN:
/// - Rigidbody2D
/// - Collider2D
/// - DamageDealer
/// - Meteor.cs
/// </summary>
public class MeteorSpawner : MonoBehaviour
{
    // =========================================================
    // ENUMS
    // =========================================================

    public enum SpawnerMode
    {
        Always,
        OnTrigger,
        TrackPlayer
    }

    public enum SpawnPositionMode
    {
        AboveViewport,
        RandomEdge,
        AroundPlayer,
        FixedZone
    }

    // =========================================================
    // PREFABS
    // =========================================================

    [Header("════════ PREFABS ════════")]

    [SerializeField]
    private GameObject meteorPrefab;

    [SerializeField]
    private GameObject warningPrefab;

    [SerializeField]
    private Transform meteorParent;

    // =========================================================
    // MODE
    // =========================================================

    [Header("════════ MODE ═══════════")]

    [SerializeField]
    private SpawnerMode spawnerMode = SpawnerMode.OnTrigger;

    [SerializeField]
    private SpawnPositionMode spawnPositionMode =
        SpawnPositionMode.AboveViewport;

    [SerializeField]
    private string playerTag = "Player";

    // =========================================================
    // SPAWN
    // =========================================================

    [Header("════════ SPAWN ══════════")]

    [SerializeField]
    private int minPerWave = 1;

    [SerializeField]
    private int maxPerWave = 4;

    [SerializeField]
    private float minWaveInterval = 1f;

    [SerializeField]
    private float maxWaveInterval = 3f;

    [SerializeField]
    private float intraWaveDelay = 0.15f;

    [SerializeField]
    private float spawnDuration = 0f;

    // =========================================================
    // WARNING
    // =========================================================

    [Header("════════ WARNING ════════")]

    [SerializeField]
    private bool useWarning = true;

    [SerializeField]
    private float warningTime = 0.8f;

    // =========================================================
    // SPEED
    // =========================================================

    [Header("════════ SPEED ══════════")]

    [SerializeField]
    private float minSpeed = 4f;

    [SerializeField]
    private float maxSpeed = 12f;

    // =========================================================
    // ANGLE
    // =========================================================

    [Header("════════ ANGLE ══════════")]

    [Tooltip("270 = xuống")]
    [SerializeField]
    private float baseAngleDeg = 270f;

    [SerializeField]
    [Range(0f, 180f)]
    private float angleSpread = 30f;

    // =========================================================
    // SIZE
    // =========================================================

    [Header("════════ SIZE ═══════════")]

    [SerializeField]
    private float minSize = 0.4f;

    [SerializeField]
    private float maxSize = 1.8f;

    // =========================================================
    // POSITION
    // =========================================================

    [Header("════════ POSITION ═══════")]

    [SerializeField]
    private float spawnMargin = 2f;

    [SerializeField]
    private float aroundPlayerRadius = 10f;

    [SerializeField]
    private Vector2 fixedZoneSize = new Vector2(12f, 5f);

    // =========================================================
    // LIMIT
    // =========================================================

    [Header("════════ LIMIT ══════════")]

    [SerializeField]
    private int maxActiveMeteors = 0;

    [SerializeField]
    private bool refreshCameraEachWave = false;

    // =========================================================
    // RUNTIME
    // =========================================================

    private Camera _cam;

    private Transform _player;

    private bool _spawning;

    private Coroutine _spawnRoutine;

    private int _activeMeteorCount;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        _cam = Camera.main;

        FindPlayer();
    }

    private void Start()
    {
        // FIXED:
        // TrackPlayer giờ auto start

        if (spawnerMode == SpawnerMode.Always ||
            spawnerMode == SpawnerMode.TrackPlayer)
        {
            StartSpawning();
        }
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spawnerMode != SpawnerMode.OnTrigger)
            return;

        if (!other.CompareTag(playerTag))
            return;

        StartSpawning();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (spawnerMode != SpawnerMode.OnTrigger)
            return;

        if (!other.CompareTag(playerTag))
            return;

        StopSpawning();
    }

    // =========================================================
    // PUBLIC API
    // =========================================================

    public void StartSpawning()
    {
        if (_spawning)
            return;

        _spawning = true;

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (!_spawning)
            return;

        _spawning = false;

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);
    }

    // =========================================================
    // MAIN LOOP
    // =========================================================

    private IEnumerator SpawnLoop()
    {
        float elapsed = 0f;

        while (_spawning)
        {
            if (refreshCameraEachWave)
                _cam = Camera.main;

            if (spawnerMode == SpawnerMode.TrackPlayer)
                FindPlayer();

            int count =
                Random.Range(minPerWave, maxPerWave + 1);

            for (int i = 0; i < count; i++)
            {
                if (maxActiveMeteors > 0 &&
                    _activeMeteorCount >= maxActiveMeteors)
                {
                    break;
                }

                yield return SpawnOneMeteor();

                if (intraWaveDelay > 0f)
                    yield return new WaitForSeconds(intraWaveDelay);
            }

            float interval =
                Random.Range(minWaveInterval, maxWaveInterval);

            yield return new WaitForSeconds(interval);

            if (spawnDuration > 0f)
            {
                elapsed += interval;

                if (elapsed >= spawnDuration)
                {
                    StopSpawning();
                }
            }
        }
    }

    // =========================================================
    // SPAWN
    // =========================================================

    private IEnumerator SpawnOneMeteor()
    {
        if (meteorPrefab == null)
            yield break;

        Vector3 spawnPos = GetSpawnPosition();

        // WARNING
        if (useWarning &&
            warningPrefab != null)
        {
            GameObject warning =
                Instantiate(
                    warningPrefab,
                    spawnPos,
                    Quaternion.identity);

            Destroy(warning, warningTime);

            yield return new WaitForSeconds(warningTime);
        }

        Vector2 velocity = GetVelocity(spawnPos);

        GameObject meteorObj =
            Instantiate(
                meteorPrefab,
                spawnPos,
                Quaternion.identity,
                meteorParent);

        Meteor meteor =
            meteorObj.GetComponent<Meteor>();

        if (meteor != null)
        {
            meteor.InitMeteor(
                velocity,
                minSize,
                maxSize);
        }
        else
        {
            Debug.LogWarning(
                "[MeteorSpawner] Meteor prefab thiếu Meteor.cs");
        }

        _activeMeteorCount++;

        MeteorDestroyCallback callback =
            meteorObj.AddComponent<MeteorDestroyCallback>();

        callback.Init(() => _activeMeteorCount--);
    }

    // =========================================================
    // POSITION
    // =========================================================

    private Vector3 GetSpawnPosition()
    {
        switch (spawnPositionMode)
        {
            case SpawnPositionMode.AboveViewport:
                return SpawnAboveViewport();

            case SpawnPositionMode.RandomEdge:
                return SpawnRandomEdge();

            case SpawnPositionMode.AroundPlayer:
                return SpawnAroundPlayer();

            case SpawnPositionMode.FixedZone:
                return SpawnInFixedZone();

            default:
                return transform.position;
        }
    }

    // =========================================================
    // PERSPECTIVE + ORTHOGRAPHIC SAFE
    // =========================================================

    private Vector3 SpawnAboveViewport()
    {
        if (_cam == null)
            return transform.position;

        float distance =
            Mathf.Abs(_cam.transform.position.z);

        Vector3 left =
            _cam.ViewportToWorldPoint(
                new Vector3(0, 1, distance));

        Vector3 right =
            _cam.ViewportToWorldPoint(
                new Vector3(1, 1, distance));

        float y = left.y + spawnMargin;

        float x =
            Random.Range(left.x, right.x);

        return new Vector3(x, y, 0f);
    }

    private Vector3 SpawnRandomEdge()
    {
        if (_cam == null)
            return transform.position;

        float distance =
            Mathf.Abs(_cam.transform.position.z);

        Vector3 topLeft =
            _cam.ViewportToWorldPoint(
                new Vector3(0, 1, distance));

        Vector3 bottomRight =
            _cam.ViewportToWorldPoint(
                new Vector3(1, 0, distance));

        float left = topLeft.x;
        float right = bottomRight.x;
        float top = topLeft.y;
        float bottom = bottomRight.y;

        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0:
                return new Vector3(
                    Random.Range(left, right),
                    top + spawnMargin,
                    0);

            case 1:
                return new Vector3(
                    Random.Range(left, right),
                    bottom - spawnMargin,
                    0);

            case 2:
                return new Vector3(
                    left - spawnMargin,
                    Random.Range(bottom, top),
                    0);

            default:
                return new Vector3(
                    right + spawnMargin,
                    Random.Range(bottom, top),
                    0);
        }
    }

    private Vector3 SpawnAroundPlayer()
    {
        Vector2 center =
            _player != null
            ? (Vector2)_player.position
            : (Vector2)transform.position;

        float angle =
            Random.Range(0f, 360f) * Mathf.Deg2Rad;

        return new Vector3(
            center.x + Mathf.Cos(angle) * aroundPlayerRadius,
            center.y + Mathf.Sin(angle) * aroundPlayerRadius,
            0f);
    }

    private Vector3 SpawnInFixedZone()
    {
        return new Vector3(
            transform.position.x +
            Random.Range(
                -fixedZoneSize.x / 2f,
                fixedZoneSize.x / 2f),

            transform.position.y +
            Random.Range(
                -fixedZoneSize.y / 2f,
                fixedZoneSize.y / 2f),

            0f);
    }

    // =========================================================
    // VELOCITY
    // =========================================================

    private Vector2 GetVelocity(Vector3 spawnPos)
    {
        float angle =
            baseAngleDeg +
            Random.Range(-angleSpread, angleSpread);

        // AUTO AIM PLAYER
        if (spawnPositionMode ==
            SpawnPositionMode.AroundPlayer &&
            _player != null)
        {
            Vector2 dir =
                ((Vector2)_player.position -
                (Vector2)spawnPos).normalized;

            float aimAngle =
                Mathf.Atan2(dir.y, dir.x) *
                Mathf.Rad2Deg;

            angle =
                aimAngle +
                Random.Range(-angleSpread, angleSpread);
        }

        float speed =
            Random.Range(minSpeed, maxSpeed);

        float rad = angle * Mathf.Deg2Rad;

        Vector2 velocity =
            new Vector2(
                Mathf.Cos(rad),
                Mathf.Sin(rad));

        return velocity * speed;
    }

    // =========================================================
    // UTIL
    // =========================================================

    private void FindPlayer()
    {
        GameObject p =
            GameObject.FindGameObjectWithTag(playerTag);

        if (p != null)
            _player = p.transform;
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        // FIXED ZONE
        if (spawnPositionMode ==
            SpawnPositionMode.FixedZone)
        {
            Gizmos.color =
                new Color(1f, 0.5f, 0f, 0.3f);

            Gizmos.DrawCube(
                transform.position,
                fixedZoneSize);

            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(
                transform.position,
                fixedZoneSize);
        }

        // AROUND PLAYER
        if (spawnPositionMode ==
            SpawnPositionMode.AroundPlayer)
        {
            Gizmos.color =
                new Color(1f, 0f, 0f, 0.3f);

            DrawCircle(
                transform.position,
                aroundPlayerRadius,
                32);
        }

        // DIRECTION
        Gizmos.color = Color.cyan;

        float rad =
            baseAngleDeg * Mathf.Deg2Rad;

        Vector3 dir =
            new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0f);

        Gizmos.DrawLine(
            transform.position,
            transform.position + dir * 3f);
    }

    private void DrawCircle(
        Vector3 center,
        float radius,
        int segments)
    {
        float step = 360f / segments;

        Vector3 prev =
            center + new Vector3(radius, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle =
                step * i * Mathf.Deg2Rad;

            Vector3 next =
                center +
                new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius);

            Gizmos.DrawLine(prev, next);

            prev = next;
        }
    }
}

// =========================================================
// DESTROY CALLBACK
// =========================================================

public class MeteorDestroyCallback : MonoBehaviour
{
    private System.Action _onDestroy;

    public void Init(System.Action onDestroy)
    {
        _onDestroy = onDestroy;
    }

    private void OnDestroy()
    {
        _onDestroy?.Invoke();
    }
}