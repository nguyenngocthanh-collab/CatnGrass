using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            int currentScene =
                SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(currentScene + 1);
        }
    }
}