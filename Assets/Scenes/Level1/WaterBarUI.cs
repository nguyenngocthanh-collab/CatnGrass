using UnityEngine;
using UnityEngine.UI;

public class WaterBarUI : MonoBehaviour
{
    [SerializeField]
    private WaterSystem waterSystem;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (waterSystem == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 1f;

        UpdateBar(
            waterSystem.CurrentWater,
            waterSystem.MaxWater);

        waterSystem.OnWaterChanged
            .AddListener(UpdateBar);
    }

    private void UpdateBar(
        float current,
        float max)
    {
        slider.value = current / max;
    }
}