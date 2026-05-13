using UnityEngine;

public class Level3FlappyPipeSpawner : MonoBehaviour
{
    [Header("Pipe Prefab")]
    [SerializeField] private GameObject pipePrefab;

    // =========================
    // SPEED
    // =========================

    [Header("Pipe Speed")]
    [SerializeField] private float startPipeSpeed = 3f;

    [SerializeField] private float speedIncreasePerSecond = 0.15f;

    [SerializeField] private float maxPipeSpeed = 10f;

    // =========================
    // DISTANCE
    // =========================

    [Header("Distance Between Pipes")]
    [SerializeField] private float distanceBetweenPipes = 8f;

    // =========================
    // HEIGHT
    // =========================

    [Header("Spawn Height")]
    [SerializeField] private float startMinY = -2f;

    [SerializeField] private float startMaxY = 2f;

    [SerializeField] private float heightExpandPerSecond = 0.1f;

    [SerializeField] private float maxHeightLimit = 5f;

    // =========================

    private float currentPipeSpeed;

    private float currentMinY;
    private float currentMaxY;

    private float spawnTimer;

    private void Start()
    {
        currentPipeSpeed = startPipeSpeed;

        currentMinY = startMinY;
        currentMaxY = startMaxY;
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

        // Spawn sync theo speed
        float currentSpawnRate =
            distanceBetweenPipes / currentPipeSpeed;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnRate)
        {
            SpawnPipe();

            spawnTimer = 0f;
        }
    }

    private void UpdateDifficulty()
    {
        // SPEED TĂNG DẦN
        currentPipeSpeed +=
            speedIncreasePerSecond * Time.deltaTime;

        currentPipeSpeed =
            Mathf.Clamp(
                currentPipeSpeed,
                0f,
                maxPipeSpeed
            );

        // HEIGHT RANGE MỞ RỘNG DẦN
        currentMaxY +=
            heightExpandPerSecond * Time.deltaTime;

        currentMinY -=
            heightExpandPerSecond * Time.deltaTime;

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
            Debug.LogError("Pipe Prefab chưa assign!");

            return;
        }

        float randomY =
            Random.Range(currentMinY, currentMaxY);

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
            pipe.GetComponent<Level3FlappyPipe>();

        if (pipeScript != null)
        {
            pipeScript.SetMoveSpeed(currentPipeSpeed);
        }
    }
}