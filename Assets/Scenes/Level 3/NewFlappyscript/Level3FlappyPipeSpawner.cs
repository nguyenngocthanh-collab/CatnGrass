using UnityEngine;

public class Level3FlappyPipeSpawner : MonoBehaviour
{
    [Header("Pipe Prefab")]

    [SerializeField]
    private GameObject pipePrefab;

    [Header("Ghost Event")]

    [SerializeField]
    private GhostEventManager ghostEventManager;

    // =========================
    // SPEED
    // =========================

    [Header("Pipe Speed")]

    [SerializeField]
    private float startPipeSpeed = 4f;

    [SerializeField]
    private float speedIncreasePerSecond = 0.07f;

    [SerializeField]
    private float maxPipeSpeed = 8f;

    // =========================
    // DISTANCE
    // =========================

    [Header("Distance Between Pipes")]

    [SerializeField]
    private float distanceBetweenPipes = 10f;

    // =========================
    // HEIGHT
    // =========================

    [Header("Spawn Height")]

    [SerializeField]
    private float startMinY = -1.8f;

    [SerializeField]
    private float startMaxY = 1.8f;

    [SerializeField]
    private float heightExpandPerSecond = 0.03f;

    [SerializeField]
    private float maxHeightLimit = 3.5f;

    // =========================

    private float currentPipeSpeed;

    private float currentMinY;

    private float currentMaxY;

    private float spawnTimer;

    // expose speed cho ghost
    public float CurrentPipeSpeed =>
        currentPipeSpeed;

    private void Start()
    {
        currentPipeSpeed =
            startPipeSpeed;

        currentMinY =
            startMinY;

        currentMaxY =
            startMaxY;
    }

    private void Update()
    {
        if (Level3FlappyGameManager.Instance == null)
            return;

        if (Level3FlappyGameManager.Instance.IsDead)
            return;

        if (!Level3FlappyGameManager.Instance.HasStarted)
            return;

        UpdateDifficulty();

        // =========================
        // BLOCK PIPE
        // =========================

        if (ghostEventManager != null &&
            ghostEventManager.BlockPipeSpawn)
        {
            spawnTimer = 0f;

            return;
        }

        // =========================
        // SPAWN
        // =========================

        float currentSpawnRate =
            distanceBetweenPipes /
            currentPipeSpeed;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnRate)
        {
            SpawnPipe();

            spawnTimer = 0f;
        }
    }

    private void UpdateDifficulty()
    {
        currentPipeSpeed +=
            speedIncreasePerSecond *
            Time.deltaTime;

        currentPipeSpeed =
            Mathf.Clamp(
                currentPipeSpeed,
                0f,
                maxPipeSpeed
            );

        currentMaxY +=
            heightExpandPerSecond *
            Time.deltaTime;

        currentMinY -=
            heightExpandPerSecond *
            Time.deltaTime;

        currentMaxY =
            Mathf.Clamp(
                currentMaxY,
                0f,
                maxHeightLimit
            );

        currentMinY =
            Mathf.Clamp(
                currentMinY,
                -maxHeightLimit,
                0f
            );
    }

    private void SpawnPipe()
    {
        if (pipePrefab == null)
        {
            Debug.LogError(
                "Pipe Prefab chưa assign!"
            );

            return;
        }

        float randomY =
            Random.Range(
                currentMinY,
                currentMaxY
            );

        Vector3 spawnPosition =
            new Vector3(
                transform.position.x,
                randomY,
                0f
            );

        GameObject pipe =
            Instantiate(
                pipePrefab,
                spawnPosition,
                Quaternion.identity
            );

        Level3FlappyPipe pipeScript =
            pipe.GetComponent<
                Level3FlappyPipe>();

        if (pipeScript != null)
        {
            pipeScript.SetMoveSpeed(
                currentPipeSpeed
            );
        }
    }
}