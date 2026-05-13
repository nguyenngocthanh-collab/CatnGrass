using UnityEngine;

public class WaterPickup : MonoBehaviour
{
    [SerializeField]
    private float waterAmount = 25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        WaterSystem water =
            other.GetComponent<WaterSystem>();

        if (water == null)
            return;

        water.AddWater(waterAmount);

        Destroy(gameObject);
    }
}