using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadLevel();
    }

    public void SkipVideo()
    {
        LoadLevel();
    }

    void LoadLevel()
    {
        SceneManager.LoadScene("PlanetSence");
    }
}