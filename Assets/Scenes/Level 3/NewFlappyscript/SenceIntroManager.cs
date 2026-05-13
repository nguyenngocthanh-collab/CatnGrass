using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneIntroManager : MonoBehaviour
{
    [Header("Fade")]

    [SerializeField]
    private Image blackImage;

    [SerializeField]
    private float fadeDuration = 2f;

    [Header("Animation")]

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string animationTrigger = "Play";

    [SerializeField]
    private float introDuration = 5f;

    private void Start()
    {
        StartCoroutine(IntroFlow());
    }

    private IEnumerator IntroFlow()
    {
        // khoá gameplay
        Time.timeScale = 0f;

        // full đen
        SetAlpha(1f);

        // play anim
        if (animator != null)
        {
            animator.updateMode =
                AnimatorUpdateMode.UnscaledTime;

            animator.SetTrigger(
                animationTrigger
            );
        }

        // fade sáng dần
        yield return StartCoroutine(
            Fade(1f, 0f)
        );

        // đợi intro
        yield return new WaitForSecondsRealtime(
            introDuration
        );

        // mở gameplay
        Time.timeScale = 1f;

        if (Level3FlappyGameManager.Instance != null)
        {
            Level3FlappyGameManager.Instance
                .StartGame();
        }
    }

    private IEnumerator Fade(
        float start,
        float end)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            float alpha =
                Mathf.Lerp(
                    start,
                    end,
                    time / fadeDuration
                );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(end);
    }

    private void SetAlpha(float alpha)
    {
        if (blackImage == null)
            return;

        Color color =
            blackImage.color;

        color.a = alpha;

        blackImage.color = color;
    }
}