using System.Collections;
using UnityEngine;

public class MenuMusicFade : MonoBehaviour
{
    public AudioSource audioSource;

    public float fadeDuration = 2f;
    public float targetVolume = 0.7f;

    private void Start()
    {
        audioSource.volume = 0f;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(0, targetVolume, time / fadeDuration);

            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(startVolume, 0, time / fadeDuration);

            yield return null;
        }

        audioSource.volume = 0;
    }
}