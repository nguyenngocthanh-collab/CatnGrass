using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPanel;

    private bool isDead = false;

    void Start()
    {
        deathPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        if (isDead) return;

        isDead = true;

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