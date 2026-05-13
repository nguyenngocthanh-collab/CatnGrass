using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3FlappyGameManager : MonoBehaviour
{
    public static Level3FlappyGameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject deathPanel;

    [Header("Death")]
    [SerializeField] private float deathDelay = 0f;

    [Header("Death Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    [Header("Timer")]
    [SerializeField] private float levelDuration = 120f;

    [Header("Ending Portal")]
    [SerializeField] private Level3FlappyEndingPortal portal;

    [SerializeField] private float portalSpawnTime = 5f;

    public bool IsPaused { get; private set; }
    public bool IsDead { get; private set; }
    public bool HasStarted { get; private set; }

    private float timer;

    private bool portalSpawned;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;

        Time.timeScale = 1f;

        timer = levelDuration;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        // hide portal
        if (portal != null)
        {
            portal.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // debug death
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerDied();
        }

        if (IsDead)
            return;

        // timer
        if (HasStarted)
        {
            timer -= Time.deltaTime;

            // spawn portal
            if (!portalSpawned &&
                timer <= portalSpawnTime)
            {
                portalSpawned = true;

                if (portal != null)
                {
                    portal.ActivatePortal();
                }
            }
        }

        // pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // =========================
    // GAME FLOW
    // =========================

    public void StartGame()
    {
        if (HasStarted)
            return;

        HasStarted = true;
    }

    public void PlayerDied()
    {
        // không cho chết khi portal hút
        if (portal != null &&
            portal.IsPulling)
        {
            return;
        }

        if (IsDead)
            return;

        IsDead = true;

        // DEATH SOUND
        if (audioSource != null &&
            deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Invoke(nameof(ShowDeathPanel), deathDelay);
    }

    private void ShowDeathPanel()
    {
        if (deathPanel == null)
            return;

        deathPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================
    // PAUSE
    // =========================

    public void TogglePause()
    {
        if (IsDead)
            return;

        IsPaused = !IsPaused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(IsPaused);
        }

        Time.timeScale =
            IsPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (IsDead)
            return;

        IsPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    // =========================
    // SCENE
    // =========================

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