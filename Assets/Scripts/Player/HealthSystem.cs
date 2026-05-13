using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component HP dùng chung cho:
/// - Player
/// - Enemy
/// - Boss
/// - Anything có máu
/// </summary>

public class HealthSystem : MonoBehaviour, IDamageable
{
    // =====================================================
    // STATS
    // =====================================================

    [Header("Stats")]
    [SerializeField] private int maxHP = 3;

    // =====================================================
    // SOUND
    // =====================================================

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;

    // =====================================================
    // EVENTS
    // =====================================================

    [Header("Events")]

    // currentHP, maxHP
    public UnityEvent<int, int> OnHPChanged;

    public UnityEvent OnDeath;

    // =====================================================
    // INTERNAL
    // =====================================================

    private int currentHP;

    private bool isDead;

    private bool invincible;

    // =====================================================
    // PROPERTIES
    // =====================================================

    public int CurrentHP => currentHP;

    public int MaxHP => maxHP;

    public bool IsDead => isDead;

    public bool IsInvincible => invincible;

    // =====================================================
    // UNITY
    // =====================================================

    private void Awake()
    {
        currentHP = maxHP;
    }

    // =====================================================
    // DAMAGE
    // =====================================================

    public void TakeDamage(int amount)
    {
        if (isDead || invincible || amount <= 0)
        {
            return;
        }

        // Hurt sound chỉ phát nếu chưa chết
        if (currentHP > 1)
        {
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }
        }

        currentHP =
            Mathf.Max(
                currentHP - amount,
                0);

        OnHPChanged?.Invoke(
            currentHP,
            maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // =====================================================
    // HEAL
    // =====================================================

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHP =
            Mathf.Min(
                currentHP + amount,
                maxHP);

        OnHPChanged?.Invoke(
            currentHP,
            maxHP);
    }

    // =====================================================
    // RESET FULL HP
    // =====================================================

    public void ResetHP()
    {
        isDead = false;

        currentHP = maxHP;

        OnHPChanged?.Invoke(
            currentHP,
            maxHP);
    }

    // =====================================================
    // SET HP
    // =====================================================

    public void SetHP(int hp)
    {
        currentHP =
            Mathf.Clamp(
                hp,
                0,
                maxHP);

        isDead = currentHP <= 0;

        OnHPChanged?.Invoke(
            currentHP,
            maxHP);

        if (isDead)
        {
            OnDeath?.Invoke();
        }
    }

    // =====================================================
    // INVINCIBLE
    // =====================================================

    public void SetInvincible(bool value)
    {
        invincible = value;
    }

    // =====================================================
    // DEATH
    // =====================================================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        OnDeath?.Invoke();
    }
}