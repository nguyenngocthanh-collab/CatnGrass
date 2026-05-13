using UnityEngine;
using System.Collections.Generic;

public class GhostSpawner : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private GhostEventManager eventManager;

    [SerializeField]
    private Level3FlappyPipeSpawner pipeSpawner;

    [Header("Ghost Prefab")]

    [SerializeField]
    private GameObject ghostPrefab;

    // =========================
    // SPAWN
    // =========================

    [Header("Spawn Settings")]

    [SerializeField]
    private float spawnInterval = 1.8f;

    // =========================
    // HEIGHT
    // =========================

    [Header("Spawn Height")]

    [SerializeField]
    private float minY = -4f;

    [SerializeField]
    private float maxY = 4f;

    // =========================
    // SAFE GAP
    // =========================

    [Header("Safe Gap")]

    [SerializeField]
    private bool useCenterSafeZone = true;

    [SerializeField]
    private float safeZoneHeight = 2f;

    // =========================
    // ANTI STACK
    // =========================

    [Header("Anti Stack")]

    [SerializeField]
    private float minVerticalDistance = 2.5f;

    [SerializeField]
    private int rememberLastSpawnCount = 5;

    // =========================

    private float spawnTimer;

    private bool wasEventActive;

    private List<float> recentSpawnY =
        new List<float>();

    private void Update()
    {
        if (eventManager == null)
            return;

        if (Level3FlappyGameManager.Instance == null)
            return;

        if (Level3FlappyGameManager.Instance.IsDead)
            return;

        if (!Level3FlappyGameManager.Instance.HasStarted)
            return;

        // =========================
        // EVENT OFF
        // =========================

        if (!eventManager.IsGhostEventActive)
        {
            spawnTimer = 0f;

            wasEventActive = false;

            return;
        }

        // =========================
        // EVENT START
        // =========================

        if (!wasEventActive)
        {
            spawnTimer = 0f;

            recentSpawnY.Clear();

            wasEventActive = true;
        }

        // =========================
        // SPAWN
        // =========================

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnGhost();

            spawnTimer = 0f;
        }
    }

    private void SpawnGhost()
    {
        if (ghostPrefab == null)
        {
            Debug.LogError(
                "Ghost Prefab chưa assign!"
            );

            return;
        }

        float randomY =
            GetValidSpawnY();

        Vector3 spawnPos =
            new Vector3(
                transform.position.x,
                randomY,
                0f
            );

        GameObject ghost =
            Instantiate(
                ghostPrefab,
                spawnPos,
                Quaternion.identity
            );

        Level3FlappyPipe movement =
            ghost.GetComponent<Level3FlappyPipe>();

        if (movement != null)
        {
            movement.SetMoveSpeed(
                pipeSpawner.CurrentPipeSpeed
            );
        }

        recentSpawnY.Add(randomY);

        if (recentSpawnY.Count >
            rememberLastSpawnCount)
        {
            recentSpawnY.RemoveAt(0);
        }
    }

    private float GetValidSpawnY()
    {
        for (int i = 0; i < 30; i++)
        {
            float y =
                Random.Range(minY, maxY);

            // SAFE GAP GIỮA MÀN
            if (useCenterSafeZone)
            {
                if (Mathf.Abs(y) <
                    safeZoneHeight)
                {
                    continue;
                }
            }

            // CHỐNG STACK
            bool tooClose = false;

            foreach (float oldY in recentSpawnY)
            {
                if (Mathf.Abs(y - oldY) <
                    minVerticalDistance)
                {
                    tooClose = true;

                    break;
                }
            }

            if (!tooClose)
            {
                return y;
            }
        }

        return Random.Range(minY, maxY);
    }
}