using UnityEngine;

public class PlatformWaterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject waterPrefab;

    [SerializeField]
    [Range(0f, 1f)]
    private float spawnChance = 0.3f;

    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 1f, 0f);

    public void TrySpawnWater(
        Transform parent)
    {
        if (waterPrefab == null)
        {
            Debug.LogError(
                "[WATER] NULL PREFAB");

            return;
        }

        float roll = Random.value;

        Debug.Log(
            "[WATER] Roll = " + roll);

        Debug.Log(
            "[WATER] Chance = " + spawnChance);

        if (roll > spawnChance)
        {
            Debug.Log(
                "[WATER] FAILED");

            return;
        }

        Debug.Log(
            "[WATER] SUCCESS");

        GameObject water =
            Instantiate(
                waterPrefab,
                parent.position + offset,
                Quaternion.identity);

        water.transform.SetParent(parent);

        water.transform.localPosition =
            offset;
    }
}