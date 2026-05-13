using UnityEngine;
using UnityEngine.Events;

public class WaterSystem : MonoBehaviour
{
    [Header("Water")]

    [SerializeField]
    private float maxWater = 100f;

    [SerializeField]
    private float drainPerSecond = 5f;

    [SerializeField]
    private bool damageWhenEmpty = true;

    [SerializeField]
    private int emptyDamage = 999;

    [SerializeField]
    private float damageInterval = 1f;

    [Header("Events")]

    public UnityEvent<float, float> OnWaterChanged;

    private float currentWater;

    private float damageTimer;

    private HealthSystem health;

    public float CurrentWater => currentWater;

    public float MaxWater => maxWater;

    private void Awake()
    {
        currentWater = maxWater;

        health = GetComponent<HealthSystem>();

        OnWaterChanged?.Invoke(
            currentWater,
            maxWater);
    }

    private void Update()
    {
        DrainWater();

        EmptyCheck();
    }

    private void DrainWater()
    {
        currentWater -=
            drainPerSecond * Time.deltaTime;

        currentWater =
            Mathf.Clamp(
                currentWater,
                0f,
                maxWater);

        OnWaterChanged?.Invoke(
            currentWater,
            maxWater);
    }

    private void EmptyCheck()
    {
        if (!damageWhenEmpty)
            return;

        if (currentWater > 0f)
            return;

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;

            if (health != null)
            {
                health.TakeDamage(emptyDamage);
            }
        }
    }

    public void AddWater(float amount)
    {
        currentWater += amount;

        currentWater =
            Mathf.Clamp(
                currentWater,
                0f,
                maxWater);

        OnWaterChanged?.Invoke(
            currentWater,
            maxWater);
    }
}