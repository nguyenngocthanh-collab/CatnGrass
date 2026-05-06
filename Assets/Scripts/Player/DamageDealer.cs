using UnityEngine;

/// <summary>
/// G?n lên b?t k? object nào gây damage: gai nh?n, vùng l?a, quái, ??n...
/// Dùng trigger collider — không c?n bi?t target là Player hay Enemy.
/// 
/// Performance notes:
/// - Dùng Trigger thay vì polling m?i frame ? zero overhead khi không có contact
/// - Tags ?? filter thay vì GetComponent m?i va ch?m
/// - Cooldown per-target dùng Dictionary nh?, không allocate liên t?c
/// </summary>
public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 0.5f; // giây gi?a các l?n damage liên ti?p
                                                          // = 0 ? damage ?úng 1 l?n khi Enter

    [Header("Filter — ?? tr?ng = damage t?t c? IDamageable")]
    [SerializeField] private string[] targetTags;   // ví d?: "Player", "Enemy"

    // Dùng Dictionary nh? ?? track cooldown theo t?ng target
    // Không dùng static/global — m?i DamageDealer qu?n lý riêng
    private System.Collections.Generic.Dictionary<Collider2D, float> cooldownMap
        = new System.Collections.Generic.Dictionary<Collider2D, float>();

    // ?? Trigger callbacks — Physics engine g?i, không poll m?i frame ??

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Stay ch? ch?y khi damageCooldown > 0 (hazard liên t?c nh? l?a, gai)
        if (damageCooldown <= 0f) return;
        TryDealDamage(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // D?n map khi target r?i vùng — tránh memory leak
        cooldownMap.Remove(other);
    }

    // ?? Core logic ???????????????????????????????????????????????

    private void TryDealDamage(Collider2D other)
    {
        if (!PassesTagFilter(other)) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null) return;

        float now = Time.time;

        if (cooldownMap.TryGetValue(other, out float lastHitTime))
        {
            if (now - lastHitTime < damageCooldown) return; // còn trong cooldown
        }

        cooldownMap[other] = now;
        target.TakeDamage(damage);
    }

    private bool PassesTagFilter(Collider2D other)
    {
        if (targetTags == null || targetTags.Length == 0) return true;
        foreach (string t in targetTags)
            if (other.CompareTag(t)) return true;
        return false;
    }
}