using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup menuGroup;

    public void PlayGame()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 2f;

            menuGroup.alpha = 1 - t;

            yield return null;
        }

        SceneManager.LoadScene("IntroSence");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}