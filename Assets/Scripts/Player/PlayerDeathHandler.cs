using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    public enum DeathMode { AnimationOnly, Respawn, GameOver }

    [Header("Mode")]
    [SerializeField] private DeathMode mode = DeathMode.AnimationOnly;

    [Header("References")]
    [SerializeField] private PlayerDead playerDead;
    [SerializeField] private HealthSystem health;

    [Header("Respawn Settings")]
    [SerializeField] private Transform checkpoint;
    [SerializeField] private float respawnDelay = 1.5f;

    void Awake()
    {
        if (health == null) health = GetComponent<HealthSystem>();
        if (playerDead == null) playerDead = GetComponent<PlayerDead>();
        health.OnDeath.AddListener(HandleDeath);
    }

    public void SwitchMode(DeathMode newMode) => mode = newMode;

    private void HandleDeath()
    {
        playerDead.TriggerDeath();
        switch (mode)
        {
            case DeathMode.Respawn: StartCoroutine(RespawnRoutine()); break;
            case DeathMode.GameOver: StartCoroutine(GameOverRoutine()); break;
        }
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (checkpoint != null) transform.position = checkpoint.position;
        playerDead.ResetAlive();
        health.ResetHP();
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private System.Collections.IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}