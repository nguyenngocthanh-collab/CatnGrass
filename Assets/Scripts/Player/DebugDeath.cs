// DebugDeath.cs — s?a l?i
using UnityEngine;

public class DebugDeath : MonoBehaviour
{
    private HealthSystem health;
    private PlayerDead death;

    void Awake()
    {
        health = GetComponent<HealthSystem>();
        death = GetComponent<PlayerDead>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            health.TakeDamage(1);   // tr? 1 máu ?úng lu?ng

        if (Input.GetKeyDown(KeyCode.L))
        {
            death.ResetAlive();
            health.ResetHP();
        }
    }
}