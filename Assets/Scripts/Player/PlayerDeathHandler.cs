using System.Collections;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Death Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    [Header("Respawn")]
    [SerializeField] private Transform checkpoint;
    [SerializeField] private float respawnDelay = 1.5f;

    private HealthSystem health;
    private PlayerDead playerDead;
    private Rigidbody2D rb;

    private void Awake()
    {
        health = GetComponent<HealthSystem>();
        playerDead = GetComponent<PlayerDead>();
        rb = GetComponent<Rigidbody2D>();

        health.OnDeath.AddListener(HandleDeath);
    }

    private void HandleDeath()
    {
        // SOUND
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // ANIMATION
        if (playerDead != null)
            playerDead.TriggerDeath();

        // RESPAWN
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (checkpoint != null)
            transform.position = checkpoint.position;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        playerDead.ResetAlive();
        health.ResetHP();
    }
}