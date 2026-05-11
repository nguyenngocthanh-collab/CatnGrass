using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============================================================
//  WavePhaseManager.cs
// ============================================================
public class WavePhaseManager : MonoBehaviour
{
    [Header("LEVEL")]
    public float levelDuration = 60f;
    public List<PhaseData> phases = new();

    [Header("PREFABS")]
    public GameObject sharkPrefab;
    public GameObject hookPrefab;   // thêm dòng này


    [Header("GLOBAL")]
    public float globalSpeedMultiplier = 1f;

    [Tooltip("Spawn ngoài viewport (world units)")]
    public float spawnMargin = 0.5f;

    [Header("━━ GIZMO PREVIEW ━━")]
    public bool showGizmos = true;
    public int previewPhaseIndex = -1;
    public float gizmoArrowLength = 5f;

    // =====================================================

    Camera _cam;
    float _camH, _camW;
    float _elapsed;
    bool _running;
    readonly HashSet<int> _triggeredPhases = new();
    [Header("DEBUG")]
    public bool showPhaseTimer = true;

    // =====================================================

    void Start()
    {
        _cam = Camera.main;
        UpdateCamBounds();
        _running = true;
    }

    void Update()
    {
        if (!_running) return;

        _elapsed += Time.deltaTime;

        for (int i = 0; i < phases.Count; i++)
        {
            if (_triggeredPhases.Contains(i)) continue;
            if (phases[i] == null) continue;

            if (_elapsed >= phases[i].startTime)
            {
                _triggeredPhases.Add(i);
                StartCoroutine(RunPhase(phases[i]));
            }
        }
    }

    // =====================================================

    IEnumerator RunPhase(PhaseData phase)
    {
        foreach (var entry in phase.hazards)
        {
            if (entry == null) continue;
            float delay = entry.spawnDelay
                + Random.Range(0f, entry.randomDelayRange);
            StartCoroutine(SpawnHazardAfterDelay(entry, delay));
        }
        yield return null;
    }

    IEnumerator SpawnHazardAfterDelay(HazardEntry entry, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        SpawnHazard(entry);
    }

    // =====================================================

    void SpawnHazard(HazardEntry e)
    {
        UpdateCamBounds();

        // Resolve spawn side
        SpawnSide side = e.spawnSide;
        if (e.randomSpawnSide
            && e.randomSidePool != null
            && e.randomSidePool.Length > 0)
        {
            side = e.randomSidePool[
                Random.Range(0, e.randomSidePool.Length)];
        }

        // ── HOOK ─────────────────────────────────────────
        if (e.type == HazardType.Hook)
        {
            SpawnHook(e);
            return;
        }

        // ── SHARK ────────────────────────────────────────
        Vector2 spawnPos = GetSpawnPosition(e, side);
        Vector2 dir = GetDirection(e, side);

        GameObject go = Instantiate(sharkPrefab, spawnPos, Quaternion.identity);

        float scale = e.randomScale
            ? Random.Range(e.minScale, e.maxScale)
            : e.minScale;
        go.transform.localScale = Vector3.one * scale;

        if (e.colliderSize != Vector2.zero)
        {
            var col = go.GetComponent<CapsuleCollider2D>();
            if (col != null)
            {
                col.size = e.colliderSize;
                col.offset = e.colliderOffset;
            }
        }

        float speed = e.randomSpeed
            ? Random.Range(e.randomSpeedMin, e.randomSpeedMax)
            : e.speed;
        speed *= globalSpeedMultiplier;

        var shark = go.GetComponent<SharkEnemy>();
        if (shark == null) return;

        shark.Init(dir, speed, e.showWarningLine, e.holdDuration, e.warningFadeOut);
    }

    // =====================================================

    void SpawnHook(HazardEntry e)
    {
        
        if (hookPrefab == null)
        {
            Debug.LogWarning("[WavePhaseManager] hookPrefab chưa gán!");
            return;
        }

        // Dùng CHUNG hệ spawn với Shark + Gizmo
        float viewportX = Mathf.Lerp(
      0f,
      1f,
      (e.sideOffset + 1f) * 0.5f
  );

        Vector3 spawnViewport = new Vector3(
            viewportX,
            1.1f,
            Mathf.Abs(0f - _cam.transform.position.z)
        );

        Vector3 worldPos =
            _cam.ViewportToWorldPoint(spawnViewport);

        Vector2 spawnPos =
            new Vector2(worldPos.x, worldPos.y);

        Debug.Log("SPAWN POS = " + spawnPos);

        // Scale
        float scale = e.randomScale
            ? Random.Range(e.minScale, e.maxScale)
            : e.minScale;

        GameObject go = Instantiate(
            hookPrefab,
            spawnPos,
            Quaternion.identity
        );

        go.transform.localScale = Vector3.one * scale;

        // Collider override
        if (e.colliderSize != Vector2.zero)
        {
            var col = go.GetComponent<CapsuleCollider2D>();

            if (col != null)
            {
                col.size = e.colliderSize;
                col.offset = e.colliderOffset;
            }
        }

        // Speed
        float castSpeed = e.randomSpeed
            ? Random.Range(e.randomSpeedMin, e.randomSpeedMax)
            : e.speed;

        castSpeed *= globalSpeedMultiplier;

        var hook = go.GetComponent<HookEnemy>();

        if (hook == null)
            return;

        hook.Init(
            castSpeed,
            e.hookBehaviour.retractSpeed,
            e.hookBehaviour.castDistance,
            e.hookBehaviour.holdDuration,
            e.showWarningLine,
            e.holdDuration,
            e.warningFadeOut
        );
    }

    // =====================================================
    // DIRECTION & POSITION
    // =====================================================

    Vector2 GetDirection(HazardEntry e, SpawnSide side)
    {
        float angle = side switch
        {
            SpawnSide.Top => -90f,
            SpawnSide.Bottom => 90f,
            SpawnSide.Left => 0f,
            SpawnSide.Right => 180f,
            _ => 0f
        };

        float offset = e.randomAimOffset
            ? Random.Range(e.randomAimMin, e.randomAimMax)
            : e.aimOffset;

        angle += offset;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    Vector2 GetSpawnPosition(HazardEntry e, SpawnSide side)
    {
        Vector3 camPos = _cam.transform.position;
        float t = e.randomSideOffset ? Random.Range(-1f, 1f) : e.sideOffset;
        float lerp = (t + 1f) * 0.5f;

        return side switch
        {
            SpawnSide.Top => new Vector2(
                Mathf.Lerp(camPos.x - _camW, camPos.x + _camW, lerp),
                camPos.y + _camH + spawnMargin),

            SpawnSide.Bottom => new Vector2(
                Mathf.Lerp(camPos.x - _camW, camPos.x + _camW, lerp),
                camPos.y - _camH - spawnMargin),

            SpawnSide.Left => new Vector2(
                camPos.x - _camW - spawnMargin,
                Mathf.Lerp(camPos.y + _camH, camPos.y - _camH, lerp)),

            _ => new Vector2(
                camPos.x + _camW + spawnMargin,
                Mathf.Lerp(camPos.y + _camH, camPos.y - _camH, lerp))
        };
    }

    // =====================================================

    void UpdateCamBounds()
    {
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;
        _camH = _cam.orthographicSize;
        _camW = _camH * _cam.aspect;
    }

    // =====================================================
    // GIZMOS
    // =====================================================
    void OnGUI()
    {
        if (!showPhaseTimer) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 32;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(
            new Rect(20, 20, 500, 60),
            $"PHASE TIME: {_elapsed:F1}s",
            style
        );
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!showGizmos || phases == null) return;

        var cam = Camera.main;
        if (cam == null) return;

        _cam  = cam;
        _camH = cam.orthographicSize;
        _camW = _camH * cam.aspect;

        for (int pi = 0; pi < phases.Count; pi++)
        {
            if (previewPhaseIndex >= 0 && pi != previewPhaseIndex) continue;

            var phase = phases[pi];
            if (phase == null || phase.hazards == null) continue;

            Color col = Color.HSVToRGB((pi * 0.37f) % 1f, 0.8f, 1f);

foreach (var e in phase.hazards)
{
    if (e == null) continue;
    if (!e.showGizmo) continue;

    SpawnSide side = e.spawnSide;

                // Tắt random tạm để preview deterministc
                bool ro = e.randomSideOffset; e.randomSideOffset = false;
                bool ra = e.randomAimOffset;  e.randomAimOffset  = false;

                Vector2 spawnPos = GetSpawnPosition(e, side);
                Vector2 dir      = GetDirection(e, side);

                e.randomSideOffset = ro;
                e.randomAimOffset  = ra;

                // Spawn point
                Gizmos.color = col;
                Gizmos.DrawWireSphere(spawnPos, 0.3f);

                // Direction arrow
                Vector3 arrowEnd = (Vector3)spawnPos + (Vector3)(dir * gizmoArrowLength);
                Gizmos.DrawLine(spawnPos, arrowEnd);

                Vector3 aR = Quaternion.Euler(0, 0,  30) * -(Vector3)dir;
                Vector3 aL = Quaternion.Euler(0, 0, -30) * -(Vector3)dir;
                Gizmos.DrawLine(arrowEnd, arrowEnd + aR * 0.6f);
                Gizmos.DrawLine(arrowEnd, arrowEnd + aL * 0.6f);

                // Label
                UnityEditor.Handles.color = col;
                UnityEditor.Handles.Label(
                    spawnPos + Vector2.up * 0.5f,
                    $"P{pi} | {side} | spd:{e.speed} | hold:{e.holdDuration}s"
                );

                // Random aim cone
                if (e.randomAimOffset)
                {
                    float baseAngle = side switch
                    {
                        SpawnSide.Top    => -90f,
                        SpawnSide.Bottom =>  90f,
                        SpawnSide.Left   =>   0f,
                        _                => 180f
                    };
                    DrawCone(spawnPos,
                        baseAngle + e.randomAimMin,
                        baseAngle + e.randomAimMax,
                        gizmoArrowLength,
                        new Color(col.r, col.g, col.b, 0.15f));
                }

                // Random side offset range
                if (e.randomSideOffset)
                {
                    Vector2 pMin = GetSpawnPositionForOffset(e, side, -1f);
                    Vector2 pMax = GetSpawnPositionForOffset(e, side,  1f);
                    Gizmos.color = new Color(col.r, col.g, col.b, 0.4f);
                    Gizmos.DrawLine(pMin, pMax);
                    Gizmos.DrawWireSphere(pMin, 0.15f);
                    Gizmos.DrawWireSphere(pMax, 0.15f);
                }
            }
        }
    }

    void DrawCone(Vector2 origin, float angleMin, float angleMax,
                  float length, Color color)
    {
        Gizmos.color = color;
        int steps    = 12;
        Vector3 prev = Vector3.zero;

        for (int i = 0; i <= steps; i++)
        {
            float a   = Mathf.Lerp(angleMin, angleMax, i / (float)steps);
            float rad = a * Mathf.Deg2Rad;
            Vector3 p = (Vector3)origin
                + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * length;

            if (i > 0)
            {
                Gizmos.DrawLine(prev, p);
                if (i == 1)     Gizmos.DrawLine(origin, prev);
                if (i == steps) Gizmos.DrawLine(origin, p);
            }
            prev = p;
        }
    }

    Vector2 GetSpawnPositionForOffset(HazardEntry e, SpawnSide side, float offset)
    {
        Vector3 camPos = _cam.transform.position;
        float lerp = (offset + 1f) * 0.5f;

        return side switch
        {
            SpawnSide.Top => new Vector2(
                Mathf.Lerp(camPos.x - _camW, camPos.x + _camW, lerp),
                camPos.y + _camH + spawnMargin),
            SpawnSide.Bottom => new Vector2(
                Mathf.Lerp(camPos.x - _camW, camPos.x + _camW, lerp),
                camPos.y - _camH - spawnMargin),
            SpawnSide.Left => new Vector2(
                camPos.x - _camW - spawnMargin,
                Mathf.Lerp(camPos.y + _camH, camPos.y - _camH, lerp)),
            _ => new Vector2(
                camPos.x + _camW + spawnMargin,
                Mathf.Lerp(camPos.y + _camH, camPos.y - _camH, lerp))
        };
    }
#endif
}