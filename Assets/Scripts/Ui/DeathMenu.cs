using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;

    [Header("Delay")]
    public float deathScreenDelay = 1.5f;

    private bool isDead = false;

    void Start()
    {
        // Reset timescale khi vào scene
        Time.timeScale = 1f;

        deathPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        if (isDead) return;

        isDead = true;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Delay realtime để không bị ảnh hưởng bởi TimeScale
        yield return new WaitForSecondsRealtime(deathScreenDelay);

        deathPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}