/// <summary>
/// Implement interface này trên b?t k? object nào có th? nh?n damage.
/// DamageDealer và hazard ch? c?n g?i TakeDamage() — không c?n bi?t target là gì.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}