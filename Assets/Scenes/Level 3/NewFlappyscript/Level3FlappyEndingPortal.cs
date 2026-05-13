// ===============================
// Level3FlappyEndingPortal.cs
// ===============================

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level3FlappyEndingPortal : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pull")]
    [SerializeField] private float pullSpeed = 15f;

    [SerializeField] private float pullDistance = 5f;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Fade")]
    [SerializeField] private Level3FlappyFade fade;

    [Header("Ending")]
    [SerializeField] private string endingScene = "Ending";

    private bool activePortal;
    private bool pulling;
    private bool loadingScene;

    public bool IsPulling => pulling;

    public void ActivatePortal()
    {
        gameObject.SetActive(true);

        activePortal = true;
    }

    private void Update()
    {
        if (!activePortal)
            return;

        // portal move
        transform.position +=
            Vector3.left *
            moveSpeed *
            Time.deltaTime;

        if (player == null)
            return;

        float dist =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // start pulling
        if (!pulling &&
            dist <= pullDistance)
        {
            pulling = true;

            // lock rigidbody
            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
                rb.simulated = false;
            }

            // disable collider
            Collider2D col =
                player.GetComponent<Collider2D>();

            if (col != null)
            {
                col.enabled = false;
            }

            // fade black
            if (fade != null)
            {
                fade.StartFade();
            }
        }

        // pull player
        if (pulling)
        {
            player.position =
                Vector3.MoveTowards(
                    player.position,
                    transform.position,
                    pullSpeed * Time.deltaTime
                );

            // load ending
            if (!loadingScene &&
                Vector2.Distance(
                    player.position,
                    transform.position) < 0.2f)
            {
                loadingScene = true;

                StartCoroutine(LoadEnding());
            }
        }
    }

    private IEnumerator LoadEnding()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(endingScene);
    }
}