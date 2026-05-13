using UnityEngine;
using UnityEngine.UI;

public class Level3FlappyFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeSpeed = 2f;

    private bool fading;

    public void StartFade()
    {
        fading = true;
    }

    private void Update()
    {
        if (!fading)
            return;

        Color c = fadeImage.color;

        c.a += fadeSpeed * Time.deltaTime;

        fadeImage.color = c;
    }
}