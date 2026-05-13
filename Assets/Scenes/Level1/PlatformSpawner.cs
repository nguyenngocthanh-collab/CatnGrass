using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawn platform theo làn (lane) với gap được tính từ tốc độ thực tế.
/// 
/// LOGIC CỐT LÕI:
///   - Chia màn hình thành N làn dọc (lanes), mỗi lần spawn chọn làn random
///   - Gap dọc giữa 2 platform liên tiếp = platformSpeed * timeBetweenSpawns
///     → đảm bảo player luôn có chỗ đứng khi platform đến tầm nhìn
///   - "Density window": đếm platform đang active trong vùng player nhìn thấy,
///     nếu < minVisiblePlatforms thì force spawn ngay
/// </summary>
public class PlatformSpawner : MonoBehaviour
{
    // ──────────────────────────────────────────────
    // INSPECTOR
    // ──────────────────────────────────────────────

    [Header("Platform Prefabs")]
    [SerializeField] private List<GameObject> platformPrefabs = new();

    [Header("Lanes")]
    [Tooltip("Số làn dọc chia đều theo chiều ngang spawn zone")]
    [SerializeField] private int laneCount = 5;

    [Tooltip("Số platform spawn mỗi lần trigger (thường để 1)")]
    [SerializeField] private int spawnAmount = 1;

    [Tooltip("Số làn được spawn cùng lúc (<=laneCount). "
           + "Tăng lên nếu muốn nhiều platform hơn mỗi wave.")]
    [SerializeField][Range(1, 5)] private int lanesPerWave = 1;

    [Header("Spawn Zone")]
    [Tooltip("Chiều rộng vùng spawn (thế giới units)")]
    [SerializeField] private float spawnZoneWidth = 18f;

    [Tooltip("Offset Y so với camera (dương = phía trên camera)")]
    [SerializeField] private float spawnOffsetY = 12f;

    [Header("Platform Movement")]
    [SerializeField] private float platformSpeed = 3f;

    [Header("Gap Safety")]
    [Tooltip("Hệ số nhân khoảng cách dọc tối thiểu. "
           + "1.0 = vừa đủ nhảy qua, 1.3 = thoải mái, 2.0 = rất thưa")]
    [SerializeField][Range(0.8f, 2.5f)] private float gapSafetyFactor = 1.2f;

    [Tooltip("Platform tối thiểu phải có trong viewport của player. "
           + "Nếu thiếu → force spawn ngay không chờ timer")]
    [SerializeField] private int minVisiblePlatforms = 2;

    [Tooltip("Chiều cao viewport để đếm platform visible (world units)")]
    [SerializeField] private float visibleWindowHeight = 10f;

    [Header("Difficulty")]
    [SerializeField] private bool increaseDifficulty = false;
    [SerializeField][Range(0f, 0.05f)] private float speedIncreasePerSecond = 0.02f;
    [SerializeField] private float maxPlatformSpeed = 6f;

    [Header("Debug")]
    [SerializeField] private bool showSpawnZone = true;
    [SerializeField] private bool showDensityWindow = true;

    // ──────────────────────────────────────────────
    // PRIVATE STATE
    // ──────────────────────────────────────────────

    private Camera cam;
    private float timer;

    /// <summary>Thời gian spawn kế tiếp (tính từ tốc độ và gap safety)</summary>
    private float spawnInterval;

    /// <summary>Track platform đang sống để đếm visible</summary>
    private List<GameObject> activePlatforms = new();

    /// <summary>Làn nào vừa được dùng gần nhất (tránh spawn cùng làn liên tiếp)</summary>
    private int lastLaneUsed = -1;

    // ──────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ──────────────────────────────────────────────

    private void Start()
    {
        cam = Camera.main;
        if (cam == null) { Debug.LogError("[PlatformSpawner] NO MAIN CAMERA"); return; }

        RecalcSpawnInterval();

        // Spawn batch đầu tiên ngay lập tức để player không đứng chờ
        for (int i = 0; i < Mathf.Max(lanesPerWave, minVisiblePlatforms); i++)
            SpawnOneLane();
    }

    private void Update()
    {
        if (cam == null) return;

        if (increaseDifficulty) UpdateDifficulty();

        // Luôn recalc interval theo tốc độ hiện tại
        RecalcSpawnInterval();

        timer += Time.deltaTime;

        // Force spawn nếu visible platforms quá ít
        bool forcedSpawn = CountVisiblePlatforms() < minVisiblePlatforms;

        if (forcedSpawn || timer >= spawnInterval)
        {
            timer = 0f;
            for (int i = 0; i < lanesPerWave; i++)
                SpawnOneLane();
        }
    }

    // ──────────────────────────────────────────────
    // SPAWN LOGIC
    // ──────────────────────────────────────────────

    /// <summary>
    /// Tính spawnInterval sao cho gap dọc giữa 2 platform = an toàn.
    /// 
    ///   gap_y = platformSpeed * spawnInterval * gapSafetyFactor
    ///   → ta muốn gap_y ≤ jumpHeight của player (khoảng 3-4 units)
    ///   → spawnInterval = jumpHeight / (platformSpeed * gapSafetyFactor)
    /// 
    /// Nhưng ta không biết jumpHeight → dùng gapSafetyFactor để người dùng tune.
    /// Mặc định: interval = 3.5 / platformSpeed * gapSafetyFactor
    /// </summary>
    private void RecalcSpawnInterval()
    {
        // 3.5f ≈ khoảng nhảy thoải mái trong units (tune theo game)
        const float referenceJumpHeight = 3.5f;
        spawnInterval = (referenceJumpHeight * gapSafetyFactor) / platformSpeed;
        spawnInterval = Mathf.Clamp(spawnInterval, 0.4f, 3f);
    }

    private void SpawnOneLane()
    {
        if (platformPrefabs.Count == 0) { Debug.LogError("[PlatformSpawner] NO PREFABS"); return; }

        int lane = PickLane();
        Vector3 pos = LaneToWorldPos(lane);

        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Count)];
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        // Set tốc độ
        FallingPlatform fp = obj.GetComponent<FallingPlatform>();
        if (fp != null) fp.SetSpeed(platformSpeed);

        activePlatforms.Add(obj);
        lastLaneUsed = lane;

        // Dọn list (remove destroyed objects)
        activePlatforms.RemoveAll(p => p == null);
    }

    /// <summary>Chọn làn ngẫu nhiên, tránh lặp làn vừa dùng</summary>
    private int PickLane()
    {
        if (laneCount <= 1) return 0;

        int lane;
        int tries = 0;
        do
        {
            lane = Random.Range(0, laneCount);
            tries++;
        }
        while (lane == lastLaneUsed && tries < 10);

        return lane;
    }

    /// <summary>Chuyển chỉ số làn → world position</summary>
    private Vector3 LaneToWorldPos(int lane)
    {
        float zoneLeft = cam.transform.position.x - spawnZoneWidth / 2f;
        float laneWidth = spawnZoneWidth / laneCount;

        // Giữa làn + jitter nhỏ (±20% laneWidth) cho tự nhiên hơn
        float jitter = Random.Range(-laneWidth * 0.2f, laneWidth * 0.2f);
        float x = zoneLeft + laneWidth * (lane + 0.5f) + jitter;
        float y = cam.transform.position.y + spawnOffsetY;

        return new Vector3(x, y, 0f);
    }

    /// <summary>Đếm platform đang nằm trong "density window" (vùng player thấy)</summary>
    private int CountVisiblePlatforms()
    {
        activePlatforms.RemoveAll(p => p == null);

        float playerY = cam.transform.position.y;
        float windowTop = playerY + visibleWindowHeight;
        float windowBot = playerY - visibleWindowHeight * 0.3f; // nhìn ít xuống dưới hơn

        int count = 0;
        foreach (var p in activePlatforms)
        {
            if (p == null) continue;
            float py = p.transform.position.y;
            if (py >= windowBot && py <= windowTop)
                count++;
        }
        return count;
    }

    // ──────────────────────────────────────────────
    // DIFFICULTY
    // ──────────────────────────────────────────────

    private void UpdateDifficulty()
    {
        platformSpeed += speedIncreasePerSecond * Time.deltaTime;
        platformSpeed = Mathf.Clamp(platformSpeed, 0f, maxPlatformSpeed);
        // spawnInterval sẽ tự recalc ở RecalcSpawnInterval() mỗi frame
    }

    // ──────────────────────────────────────────────
    // GIZMOS
    // ──────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        Camera debugCam = Camera.main;
        if (debugCam == null) return;

        if (showSpawnZone)
        {
            Gizmos.color = Color.green;
            Vector3 spawnCenter = new(
                debugCam.transform.position.x,
                debugCam.transform.position.y + spawnOffsetY,
                0f);
            Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnZoneWidth, 0.5f, 0f));

            // Vẽ đường chia làn
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            float laneWidth = spawnZoneWidth / Mathf.Max(laneCount, 1);
            float left = debugCam.transform.position.x - spawnZoneWidth / 2f;
            for (int i = 0; i <= laneCount; i++)
            {
                float x = left + laneWidth * i;
                Gizmos.DrawLine(
                    new Vector3(x, spawnCenter.y - 0.5f, 0),
                    new Vector3(x, spawnCenter.y + 0.5f, 0));
            }
        }

        if (showDensityWindow)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
            float windowCenterY = debugCam.transform.position.y + visibleWindowHeight * 0.35f;
            Gizmos.DrawCube(
                new Vector3(debugCam.transform.position.x, windowCenterY, 0f),
                new Vector3(spawnZoneWidth, visibleWindowHeight * 1.3f, 0f));
        }
    }
}