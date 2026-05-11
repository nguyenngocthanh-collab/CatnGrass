using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    public GameObject startButton;

    public void ShowStartButton()
    {
        startButton.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("lv01_v01");
    }
}