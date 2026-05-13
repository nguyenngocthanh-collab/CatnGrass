using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LavaLevelTimer : MonoBehaviour
{
    // =====================================================
    // TIMER
    // =====================================================

    [Header("════════ TIMER ════════")]

    [SerializeField]
    private float levelDuration = 60f;

    private float timer;

    private bool levelEnded;

    // =====================================================
    // PLAYER
    // =====================================================

    [Header("════════ PLAYER ═══════")]

    [SerializeField]
    private HealthSystem playerHealth;

    // =====================================================
    // METEOR EVENT
    // =====================================================

    [Header("════════ METEOR EVENT ═══════")]

    [SerializeField]
    private MeteorSpawner meteorSpawner;

    [SerializeField]
    private LavaMeteorEvent[] meteorEvents;

    // =====================================================
    // SCENE
    // =====================================================

    [Header("════════ SCENE ════════")]

    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private float sceneLoadDelay = 2f;

    // =====================================================
    // FADE
    // =====================================================

    [Header("════════ FADE ═════════")]

    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private float fadeDuration = 1.5f;

    // =====================================================
    // UNITY
    // =====================================================

    private void Start()
    {
        timer = levelDuration;

        if (meteorSpawner != null)
        {
            meteorSpawner.StopSpawning();
        }

        StartCoroutine(EventRoutine());
    }

    private void Update()
    {
        if (levelEnded)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndLevel();
        }
    }

    // =====================================================
    // EVENT ROUTINE
    // =====================================================

    private IEnumerator EventRoutine()
    {
        foreach (LavaMeteorEvent e in meteorEvents)
        {
            yield return new WaitForSeconds(e.startTime);

            if (meteorSpawner == null)
                yield break;

            ApplyDifficulty(e);

            meteorSpawner.StartSpawning();

            yield return new WaitForSeconds(e.duration);

            meteorSpawner.StopSpawning();
        }
    }

    // =====================================================
    // DIFFICULTY
    // =====================================================

    private void ApplyDifficulty(LavaMeteorEvent e)
    {
        meteorSpawner.SetDifficulty(
            e.minPerWave,
            e.maxPerWave,
            e.minInterval,
            e.maxInterval,
            e.minSpeed,
            e.maxSpeed
        );
    }

    // =====================================================
    // END LEVEL
    // =====================================================

    private void EndLevel()
    {
        if (levelEnded)
            return;

        levelEnded = true;

        if (playerHealth != null)
        {
            playerHealth.SetInvincible(true);
        }

        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        float t = 0f;

        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = t / fadeDuration;

            fadeImage.color =
                new Color(
                    c.r,
                    c.g,
                    c.b,
                    alpha);

            yield return null;
        }

        yield return new WaitForSeconds(sceneLoadDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}

[System.Serializable]
public class LavaMeteorEvent
{
    [Header("TIME")]

    public float startTime = 5f;

    public float duration = 10f;

    [Header("SPAWN")]

    public int minPerWave = 1;

    public int maxPerWave = 3;

    public float minInterval = 1f;

    public float maxInterval = 2f;

    [Header("SPEED")]

    public float minSpeed = 5f;

    public float maxSpeed = 10f;
}