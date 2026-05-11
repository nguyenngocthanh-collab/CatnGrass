using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    [Header("TIME")]
    public float levelTime = 60f;

    [Header("NEXT SCENE")]
    public string nextSceneName;

    [Header("FADE")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    bool ending = false;

    void Update()
    {
        if (ending) return;

        levelTime -= Time.deltaTime;

        // Hết giờ
        if (levelTime <= 0f)
        {
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator EndLevel()
    {
        ending = true;

        yield return StartCoroutine(FadeBlack());

        SceneManager.LoadScene(nextSceneName);
    }

    public void RestartLevel()
    {
        if (ending) return;

        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        ending = true;

        yield return StartCoroutine(FadeBlack());

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    IEnumerator FadeBlack()
    {
        float t = 0f;

        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float a = t / fadeDuration;

            fadeImage.color =
                new Color(c.r, c.g, c.b, a);

            yield return null;
        }

        fadeImage.color =
            new Color(c.r, c.g, c.b, 1f);
    }
}