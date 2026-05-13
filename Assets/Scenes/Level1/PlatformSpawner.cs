using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Platform Prefabs")]
    [SerializeField]
    private List<GameObject> platformPrefabs = new();

    [Header("Lanes")]
    [SerializeField]
    private int laneCount = 5;

    [SerializeField]
    private int spawnAmount = 1;

    [SerializeField]
    [Range(1, 5)]
    private int lanesPerWave = 1;

    [Header("Spawn Zone")]
    [SerializeField]
    private float spawnZoneWidth = 18f;

    [SerializeField]
    private float spawnOffsetY = 12f;

    [Header("Platform Movement")]
    [SerializeField]
    private float platformSpeed = 3f;

    [Header("Gap Safety")]
    [SerializeField]
    [Range(0.8f, 2.5f)]
    private float gapSafetyFactor = 1.2f;

    [SerializeField]
    private int minVisiblePlatforms = 2;

    [SerializeField]
    private float visibleWindowHeight = 10f;

    [Header("Difficulty")]
    [SerializeField]
    private bool increaseDifficulty = false;

    [SerializeField]
    [Range(0f, 0.05f)]
    private float speedIncreasePerSecond = 0.02f;

    [SerializeField]
    private float maxPlatformSpeed = 6f;

    [Header("Debug")]
    [SerializeField]
    private bool showSpawnZone = true;

    [SerializeField]
    private bool showDensityWindow = true;

    private Camera cam;

    private float timer;

    private float spawnInterval;

    private List<GameObject> activePlatforms = new();

    private int lastLaneUsed = -1;

    private void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError(
                "[PlatformSpawner] NO MAIN CAMERA");

            return;
        }

        RecalcSpawnInterval();

        for (int i = 0;
             i < Mathf.Max(
                 lanesPerWave,
                 minVisiblePlatforms);
             i++)
        {
            SpawnOneLane();
        }
    }

    private void Update()
    {
        if (cam == null)
            return;

        if (increaseDifficulty)
        {
            UpdateDifficulty();
        }

        RecalcSpawnInterval();

        timer += Time.deltaTime;

        bool forcedSpawn =
            CountVisiblePlatforms()
            < minVisiblePlatforms;

        if (forcedSpawn ||
            timer >= spawnInterval)
        {
            timer = 0f;

            for (int i = 0;
                 i < lanesPerWave;
                 i++)
            {
                SpawnOneLane();
            }
        }
    }

    private void RecalcSpawnInterval()
    {
        const float referenceJumpHeight = 3.5f;

        spawnInterval =
            (referenceJumpHeight
            * gapSafetyFactor)
            / platformSpeed;

        spawnInterval =
            Mathf.Clamp(
                spawnInterval,
                0.4f,
                3f);
    }

    private void SpawnOneLane()
    {
        if (platformPrefabs.Count == 0)
        {
            Debug.LogError(
                "[PlatformSpawner] NO PREFABS");

            return;
        }

        int lane = PickLane();

        Vector3 pos =
            LaneToWorldPos(lane);

        GameObject prefab =
            platformPrefabs[
                Random.Range(
                    0,
                    platformPrefabs.Count)];

        // DEBUG PREFAB
        Debug.Log(
            "[PLATFORM SPAWNER] PREFAB = "
            + prefab.name);

        GameObject obj =
            Instantiate(
                prefab,
                pos,
                Quaternion.identity);

        Debug.Log(
            "[PLATFORM SPAWNER] CREATED = "
            + obj.name);

        FallingPlatform fp =
            obj.GetComponent<FallingPlatform>();

        if (fp != null)
        {
            fp.SetSpeed(platformSpeed);

            Debug.Log(
                "[PLATFORM SPAWNER] FallingPlatform FOUND");
        }
        else
        {
            Debug.LogWarning(
                "[PLATFORM SPAWNER] NO FallingPlatform");
        }

        PlatformWaterSpawner waterSpawner =
            obj.GetComponent<PlatformWaterSpawner>();

        if (waterSpawner != null)
        {
            Debug.Log(
                "[PLATFORM SPAWNER] WaterSpawner FOUND");

            waterSpawner.TrySpawnWater(
                obj.transform);
        }
        else
        {
            Debug.LogWarning(
                "[PLATFORM SPAWNER] NO WaterSpawner");
        }

        activePlatforms.Add(obj);

        lastLaneUsed = lane;

        activePlatforms.RemoveAll(
            p => p == null);
    }

    private int PickLane()
    {
        if (laneCount <= 1)
            return 0;

        int lane;

        int tries = 0;

        do
        {
            lane =
                Random.Range(
                    0,
                    laneCount);

            tries++;
        }
        while (lane == lastLaneUsed
            && tries < 10);

        return lane;
    }

    private Vector3 LaneToWorldPos(
        int lane)
    {
        float zoneLeft =
            cam.transform.position.x
            - spawnZoneWidth / 2f;

        float laneWidth =
            spawnZoneWidth / laneCount;

        float jitter =
            Random.Range(
                -laneWidth * 0.2f,
                 laneWidth * 0.2f);

        float x =
            zoneLeft
            + laneWidth
            * (lane + 0.5f)
            + jitter;

        float y =
            cam.transform.position.y
            + spawnOffsetY;

        return new Vector3(
            x,
            y,
            0f);
    }

    private int CountVisiblePlatforms()
    {
        activePlatforms.RemoveAll(
            p => p == null);

        float playerY =
            cam.transform.position.y;

        float windowTop =
            playerY + visibleWindowHeight;

        float windowBot =
            playerY
            - visibleWindowHeight * 0.3f;

        int count = 0;

        foreach (var p in activePlatforms)
        {
            if (p == null)
                continue;

            float py =
                p.transform.position.y;

            if (py >= windowBot
             && py <= windowTop)
            {
                count++;
            }
        }

        return count;
    }

    private void UpdateDifficulty()
    {
        platformSpeed +=
            speedIncreasePerSecond
            * Time.deltaTime;

        platformSpeed =
            Mathf.Clamp(
                platformSpeed,
                0f,
                maxPlatformSpeed);
    }
}