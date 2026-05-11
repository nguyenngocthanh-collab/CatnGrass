using System.Collections;
using UnityEngine;

public class RespawnSystem02 : MonoBehaviour
{
    public static RespawnSystem02 Instance { get; private set; }

    [Header("Ground")]
    [SerializeField] private LayerMask safeGroundLayer;

    [Header("Hazard")]
    [SerializeField] private LayerMask hazardLayer;

    [Header("Default Spawn")]
    [SerializeField] private Transform defaultSpawnPoint;

    [Header("Safe Point")]
    [SerializeField] private float safePointUpdateInterval = 0.5f;

    [SerializeField] private float stabilityTime = 0.3f;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 0.2f;

    [SerializeField] private float invincibleAfterRespawn = 2f;

    private Vector3 _safePosition;
    private Vector3 _deathPosition;

    private float _stabilityTimer;

    private bool _isRespawning;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    private HealthSystem _health;
    private PlayerDead _playerDead;

    private playermovetest2 _controller;

    // =====================================================
    // INIT
    // =====================================================

    private void Awake()
    {
        Debug.Log("[Respawn] Awake");

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Respawn] Duplicate Instance Destroyed");

            Destroy(this);
            return;
        }

        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        _health = GetComponent<HealthSystem>();
        _playerDead = GetComponent<PlayerDead>();

        _controller = GetComponent<playermovetest2>();

        Debug.Log("[Respawn] Components Loaded");
    }

    private void Start()
    {
        _safePosition =
            defaultSpawnPoint != null
            ? defaultSpawnPoint.position
            : transform.position;

        Debug.Log("[Respawn] Initial Safe Position = " + _safePosition);

        if (_playerDead != null)
        {
            _playerDead.OnDeathAnimationComplete
                .AddListener(OnDeathAnimFinished);

            Debug.Log("[Respawn] Death Listener Added");
        }
        else
        {
            Debug.LogWarning("[Respawn] PlayerDead Missing");
        }

        InvokeRepeating(
            nameof(TryUpdateSafePoint),
            0f,
            safePointUpdateInterval);

        Debug.Log("[Respawn] SafePoint updater started");
    }

    private void OnDestroy()
    {
        if (_playerDead != null)
        {
            _playerDead.OnDeathAnimationComplete
                .RemoveListener(OnDeathAnimFinished);
        }
    }

    // =====================================================
    // SAFE POINT
    // =====================================================

    private void TryUpdateSafePoint()
    {
        if (_isRespawning)
            return;

        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                Vector2.down,
                1.5f,
                safeGroundLayer);

        if (hit.collider == null)
        {
            return;
        }

        bool insideHazard =
            Physics2D.OverlapCircle(
                transform.position,
                0.2f,
                hazardLayer);

        if (insideHazard)
        {
            Debug.Log("[Respawn] Inside Hazard -> Safe Point Blocked");
            return;
        }

        _stabilityTimer += safePointUpdateInterval;

        if (_stabilityTimer >= stabilityTime)
        {
            _safePosition = transform.position;

            Debug.Log("[Respawn] Safe Point Updated = " + _safePosition);

            _stabilityTimer = 0f;
        }
    }

    // =====================================================
    // NORMAL DEATH
    // =====================================================

    private void OnDeathAnimFinished()
    {
        Debug.Log("[Respawn] Death Animation Finished");

        if (_isRespawning)
        {
            Debug.Log("[Respawn] Already Respawning");
            return;
        }

        _deathPosition = transform.position;

        ForceRespawn(_deathPosition);
    }

    // =====================================================
    // FORCE RESPAWN
    // =====================================================

    public void ForceRespawn(Vector3 deathPos)
    {
        Debug.Log("[Respawn] ForceRespawn CALLED");

        if (_isRespawning)
        {
            Debug.Log("[Respawn] Already Respawning");
            return;
        }

        _isRespawning = true;

        _deathPosition = deathPos;

        Debug.Log("[Respawn] Death Position = " + deathPos);

        // freeze physics
        if (_rb != null)
        {
            Debug.Log("[Respawn] Freeze Rigidbody");

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            _rb.simulated = false;
        }
        else
        {
            Debug.LogError("[Respawn] Rigidbody Missing");
        }

        // disable movement
        if (_controller != null)
        {
            Debug.Log("[Respawn] Disable Controller");

            _controller.enabled = false;
        }
        else
        {
            Debug.LogWarning("[Respawn] Controller Missing");
        }

        StartCoroutine(RespawnRoutine());
    }

    // =====================================================
    // RESPAWN ROUTINE
    // =====================================================

    private IEnumerator RespawnRoutine()
    {
        Debug.Log("[Respawn] RespawnRoutine START");

        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("[Respawn] Delay Finished");

        Vector3 respawnPos = GetRespawnPosition();

        Debug.Log("[Respawn] Respawn Position = " + respawnPos);

        // teleport
        transform.position = respawnPos;

        Debug.Log("[Respawn] TELEPORTED");

        // restore physics
        if (_rb != null)
        {
            _rb.simulated = true;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            Debug.Log("[Respawn] Rigidbody Restored");
        }

        // reset health
        if (_health != null)
        {
            _health.ResetHP();

            Debug.Log("[Respawn] HP Reset");
        }
        else
        {
            Debug.LogError("[Respawn] HealthSystem Missing");
        }

        // reset death state
        if (_playerDead != null)
        {
            _playerDead.ResetAlive();

            Debug.Log("[Respawn] PlayerDead Reset");
        }
        else
        {
            Debug.LogWarning("[Respawn] PlayerDead Missing");
        }

        // enable movement
        if (_controller != null)
        {
            _controller.enabled = true;

            Debug.Log("[Respawn] Controller Enabled");
        }

        Debug.Log("[Respawn] Invincible Start");

        yield return StartCoroutine(
            InvincibleRoutine(invincibleAfterRespawn));

        Debug.Log("[Respawn] RESPAWN COMPLETE");

        _isRespawning = false;
    }

    // =====================================================
    // GET RESPAWN POSITION
    // =====================================================

    private Vector3 GetRespawnPosition()
    {
        Debug.Log("[Respawn] GetRespawnPosition");

        return _safePosition;
    }

    // =====================================================
    // INVINCIBLE
    // =====================================================

    private IEnumerator InvincibleRoutine(float duration)
    {
        Debug.Log("[Respawn] Invincible Routine START");

        if (_health != null)
        {
            _health.enabled = false;
        }

        float t = 0f;

        while (t < duration)
        {
            if (_sr != null)
            {
                _sr.enabled = !_sr.enabled;
            }

            yield return new WaitForSeconds(0.15f);

            t += 0.15f;
        }

        if (_sr != null)
        {
            _sr.enabled = true;
        }

        if (_health != null)
        {
            _health.enabled = true;
        }

        Debug.Log("[Respawn] Invincible Routine END");
    }

    // =====================================================
    // MANUAL SAFE POINT
    // =====================================================

    public void SetSafePoint(Vector3 pos)
    {
        _safePosition = pos;

        Debug.Log("[Respawn] Manual Safe Point = " + pos);
    }

    // =====================================================
    // DEBUG GIZMO
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(_safePosition, 0.2f);
    }
}