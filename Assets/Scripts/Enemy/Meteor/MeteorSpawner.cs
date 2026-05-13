using UnityEngine;
using System.Collections;

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
    // DEBUG
    // =========================================================

    [Header("════════ DEBUG ══════════")]

    [SerializeField]
    private bool enableDebugLogs = true;

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
        DebugLog("AWAKE");

        _cam = Camera.main;

        if (_cam == null)
        {
            Debug.LogError("[MeteorSpawner] Main Camera NOT FOUND");
        }
        else
        {
            DebugLog("Main Camera Found");
        }

        FindPlayer();
    }

    private void Start()
    {
        DebugLog("START");

        if (meteorPrefab == null)
        {
            Debug.LogError("[MeteorSpawner] Meteor Prefab MISSING");
        }

        if (spawnerMode == SpawnerMode.Always ||
            spawnerMode == SpawnerMode.TrackPlayer)
        {
            DebugLog("Auto Start Spawning");

            StartSpawning();
        }
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        DebugLog("Trigger Enter: " + other.name);

        if (spawnerMode != SpawnerMode.OnTrigger)
        {
            DebugLog("Not OnTrigger Mode");
            return;
        }

        Transform root = other.transform.root;

        DebugLog("Root Object: " + root.name);

        if (!root.CompareTag(playerTag))
        {
            DebugLog("Wrong Tag");
            return;
        }

        DebugLog("PLAYER DETECTED");

        StartSpawning();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        DebugLog("Trigger Exit: " + other.name);

        if (spawnerMode != SpawnerMode.OnTrigger)
            return;

        Transform root = other.transform.root;

        if (!root.CompareTag(playerTag))
            return;

        DebugLog("STOP SPAWNING");

        StopSpawning();
    }

    // =========================================================
    // PUBLIC API
    // =========================================================

    public void StartSpawning()
    {
        DebugLog("StartSpawning CALLED");

        if (_spawning)
        {
            DebugLog("Already Spawning");
            return;
        }

        _spawning = true;

        DebugLog("Spawn Loop Started");

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        DebugLog("StopSpawning CALLED");

        if (!_spawning)
        {
            DebugLog("Already Stopped");
            return;
        }

        _spawning = false;

        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);

            DebugLog("Spawn Coroutine Stopped");
        }
    }

    // =========================================================
    // MAIN LOOP
    // =========================================================

    private IEnumerator SpawnLoop()
    {
        DebugLog("SpawnLoop START");

        float elapsed = 0f;

        while (_spawning)
        {
            if (refreshCameraEachWave)
            {
                _cam = Camera.main;

                DebugLog("Camera Refreshed");
            }

            if (spawnerMode == SpawnerMode.TrackPlayer)
            {
                FindPlayer();
            }

            int count =
                Random.Range(minPerWave, maxPerWave + 1);

            DebugLog("Wave Count = " + count);

            for (int i = 0; i < count; i++)
            {
                if (maxActiveMeteors > 0 &&
                    _activeMeteorCount >= maxActiveMeteors)
                {
                    DebugLog("Meteor Limit Reached");

                    break;
                }

                yield return SpawnOneMeteor();

                if (intraWaveDelay > 0f)
                {
                    yield return new WaitForSeconds(intraWaveDelay);
                }
            }

            float interval =
                Random.Range(minWaveInterval, maxWaveInterval);

            DebugLog("Next Wave In: " + interval);

            yield return new WaitForSeconds(interval);

            if (spawnDuration > 0f)
            {
                elapsed += interval;

                if (elapsed >= spawnDuration)
                {
                    DebugLog("Spawn Duration Ended");

                    StopSpawning();
                }
            }
        }

        DebugLog("SpawnLoop END");
    }

    // =========================================================
    // SPAWN
    // =========================================================

    private IEnumerator SpawnOneMeteor()
    {
        DebugLog("SpawnOneMeteor");

        if (meteorPrefab == null)
        {
            Debug.LogError("[MeteorSpawner] Meteor Prefab NULL");

            yield break;
        }

        Vector3 spawnPos = GetSpawnPosition();

        DebugLog("Spawn Position = " + spawnPos);

        Vector2 velocity = GetVelocity(spawnPos);

        DebugLog("Velocity = " + velocity);

        GameObject meteorObj = Instantiate(
            meteorPrefab,
            spawnPos,
            Quaternion.identity,
            meteorParent);

        if (meteorObj == null)
        {
            Debug.LogError("[MeteorSpawner] Instantiate FAILED");

            yield break;
        }

        DebugLog("Meteor Spawned: " + meteorObj.name);

        Meteor meteor = meteorObj.GetComponent<Meteor>();

        if (meteor != null)
        {
            meteor.InitMeteor(
                velocity,
                minSize,
                maxSize);

            DebugLog("Meteor Initialized");
        }
        else
        {
            Debug.LogWarning(
                "[MeteorSpawner] Missing Meteor.cs");
        }

        _activeMeteorCount++;

        DebugLog("Active Meteors = " + _activeMeteorCount);

        MeteorDestroyCallback cb =
            meteorObj.AddComponent<MeteorDestroyCallback>();

        cb.Init(() =>
        {
            _activeMeteorCount--;

            DebugLog("Meteor Destroyed");

            DebugLog("Active Meteors = " + _activeMeteorCount);
        });

        yield return null;
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
    // SPAWN MODES
    // =========================================================

    private Vector3 SpawnAboveViewport()
    {
        if (_cam == null)
        {
            Debug.LogError("[MeteorSpawner] Camera NULL");

            return transform.position;
        }

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
        {
            _player = p.transform;

            DebugLog("Player Found");
        }
        else
        {
            Debug.LogWarning(
                "[MeteorSpawner] Player NOT FOUND");
        }
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void DebugLog(string msg)
    {
        if (!enableDebugLogs)
            return;

        Debug.Log("[MeteorSpawner] " + msg);
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            fixedZoneSize);
    }
    public void SetDifficulty(
    int newMinPerWave,
    int newMaxPerWave,
    float newMinInterval,
    float newMaxInterval,
    float newMinSpeed,
    float newMaxSpeed)
    {
        minPerWave = newMinPerWave;
        maxPerWave = newMaxPerWave;

        minWaveInterval = newMinInterval;
        maxWaveInterval = newMaxInterval;

        minSpeed = newMinSpeed;
        maxSpeed = newMaxSpeed;
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