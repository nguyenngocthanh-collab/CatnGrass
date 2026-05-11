using UnityEngine;

public class MousePlayerDeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthSystem health;

    [Header("Visuals")]
    [SerializeField] private GameObject aliveVisual;

    [SerializeField] private GameObject deathVisual;

    [Header("UI")]
    [SerializeField] private DeathMenu deathMenu;

    private bool isDead = false;

    private void Awake()
    {
        Debug.Log("DeathHandler Awake");

        if (health == null)
        {
            Debug.LogError("Health NULL");
            return;
        }

        health.OnDeath.AddListener(HandleDeath);

        Debug.Log("Listener Added");
    }

    private void HandleDeath()
    {
        Debug.Log("HANDLE DEATH CALLED");

        if (isDead)
        {
            Debug.Log("Already Dead");
            return;
        }

        isDead = true;

        if (aliveVisual == null)
        {
            Debug.LogError("AliveVisual NULL");
            return;
        }

        if (deathVisual == null)
        {
            Debug.LogError("DeathVisual NULL");
            return;
        }

        aliveVisual.SetActive(false);

        deathVisual.transform.position =
            aliveVisual.transform.position;

        deathVisual.transform.localScale =
            aliveVisual.transform.localScale;

        deathVisual.SetActive(true);

        Debug.Log("Death Visual Enabled");

        if (deathMenu == null)
        {
            Debug.LogError("DeathMenu NULL");
            return;
        }

        Debug.Log("Showing Death Screen");

        deathMenu.ShowDeathScreen();
    }
}