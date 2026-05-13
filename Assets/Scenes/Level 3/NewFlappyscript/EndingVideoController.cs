using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private string nextScene = "MainMenu";

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);
    }
}