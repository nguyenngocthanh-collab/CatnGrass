using UnityEngine;

public class RandomAmbientSpawner : MonoBehaviour
{
    [Header("Prefabs")]

    [SerializeField]
    private GameObject[] prefabs;

    // =========================
    // SPAWN
    // =========================

    [Header("Spawn")]

    [SerializeField]
    private float spawnInterval = 1f;

    [SerializeField]
    private int maxAlive = 20;

    // =========================
    // SCALE
    // =========================

    [Header("Scale")]

    [SerializeField]
    private float minScale = 1f;

    [SerializeField]
    private float maxScale = 2f;

    // =========================
    // SPEED
    // =========================

    [Header("Movement")]

    [SerializeField]
    private float minSpeed = 1f;

    [SerializeField]
    private float maxSpeed = 3f;

    // =========================
    // BORDER
    // =========================

    [Header("Border")]

    [SerializeField]
    private float outsideOffset = 2f;

    // =========================

    private float timer;

    private Camera cam;

    private LevelTimer levelTimer;

    private void Start()
    {
        cam = Camera.main;

        levelTimer =
            FindObjectOfType<LevelTimer>();

        if (cam == null)
        {
            Debug.LogError(
                "Không tìm thấy Main Camera"
            );
        }
    }

    private void Update()
    {
        if (cam == null)
            return;

        // =========================
        // LEVEL END
        // =========================

        if (levelTimer != null)
        {
            if (levelTimer.levelTime <= 0f)
            {
                return;
            }
        }

        // =========================
        // PREFABS
        // =========================

        if (prefabs.Length == 0)
            return;

        // =========================
        // LIMIT
        // =========================

        int aliveCount =
            GameObject.FindGameObjectsWithTag(
                "Ambient"
            ).Length;

        if (aliveCount >= maxAlive)
            return;

        // =========================
        // TIMER
        // =========================

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnAmbient();

            timer = 0f;
        }
    }

    private void SpawnAmbient()
    {
        GameObject prefab =
            prefabs[
                Random.Range(
                    0,
                    prefabs.Length
                )
            ];

        if (prefab == null)
            return;

        // =========================
        // VIEWPORT
        // =========================

        float distance =
            Mathf.Abs(
                transform.position.z -
                cam.transform.position.z
            );

        Vector3 min =
            cam.ViewportToWorldPoint(
                new Vector3(
                    0,
                    0,
                    distance
                )
            );

        Vector3 max =
            cam.ViewportToWorldPoint(
                new Vector3(
                    1,
                    1,
                    distance
                )
            );

        // =========================
        // RANDOM SIDE
        // =========================

        Vector3 spawnPos;

        Vector2 direction;

        int side =
            Random.Range(0, 4);

        switch (side)
        {
            // LEFT
            case 0:

                spawnPos =
                    new Vector3(
                        min.x - outsideOffset,
                        Random.Range(
                            min.y,
                            max.y
                        ),
                        0f
                    );

                direction =
                    Vector2.right;

                break;

            // RIGHT
            case 1:

                spawnPos =
                    new Vector3(
                        max.x + outsideOffset,
                        Random.Range(
                            min.y,
                            max.y
                        ),
                        0f
                    );

                direction =
                    Vector2.left;

                break;

            // TOP
            case 2:

                spawnPos =
                    new Vector3(
                        Random.Range(
                            min.x,
                            max.x
                        ),
                        max.y + outsideOffset,
                        0f
                    );

                direction =
                    Vector2.down;

                break;

            // BOTTOM
            default:

                spawnPos =
                    new Vector3(
                        Random.Range(
                            min.x,
                            max.x
                        ),
                        min.y - outsideOffset,
                        0f
                    );

                direction =
                    Vector2.up;

                break;
        }

        // =========================
        // SPAWN
        // =========================

        GameObject obj =
            Instantiate(
                prefab,
                spawnPos,
                Quaternion.identity
            );

        obj.tag = "Ambient";

        // =========================
        // SCALE
        // =========================

        float scale =
            Random.Range(
                minScale,
                maxScale
            );

        obj.transform.localScale =
            Vector3.one * scale;

        // =========================
        // MOVE
        // =========================

        RandomAmbientMove move =
            obj.GetComponent<
                RandomAmbientMove>();

        if (move != null)
        {
            float speed =
                Random.Range(
                    minSpeed,
                    maxSpeed
                );

            move.SetMoveData(
                direction,
                speed
            );
        }
    }
}