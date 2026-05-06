using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component chung — g?n lên Player, Enemy, hay b?t k? object nào có HP.
/// Không ch?a logic ch?t; delegate sang DeathHandler ?? d? swap.
/// </summary>
public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 3;

    [Header("Events — kéo listener vào ?ây trong Inspector")]
    public UnityEvent<int, int> OnHPChanged;   // (currentHP, maxHP)
    public UnityEvent OnDeath;

    private int currentHP;
    private bool isDead;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public bool IsDead => isDead;

    void Awake() => currentHP = maxHP;

    // ?? Public API ???????????????????????????????????????????????

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHP = Mathf.Max(currentHP - amount, 0);
        OnHPChanged.Invoke(currentHP, maxHP);

        if (currentHP == 0) Die();
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged.Invoke(currentHP, maxHP);
    }

    public void ResetHP()
    {
        isDead = false;
        currentHP = maxHP;
        OnHPChanged.Invoke(currentHP, maxHP);
    }

    // ?? Internal ?????????????????????????????????????????????????

    private void Die()
    {
        isDead = true;
        OnDeath.Invoke();
    }
}