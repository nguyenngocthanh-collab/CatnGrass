using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDead : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject aliveVisual;
    [SerializeField] private GameObject deathVisual;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Death Animation")]
    [SerializeField] private Animator deathAnimator;

    public UnityEvent OnDeathStarted;
    public UnityEvent OnDeathAnimationComplete;

    private bool isDead = false;

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        playerAnimator.DisableAnimator();
        aliveVisual.SetActive(false);

        OnDeathStarted.Invoke();

        // Coroutine ch?y trên THIS (IMG_1443 v?n active)
        // DeathVisual b?t bên trong coroutine sau 1 frame
        StartCoroutine(PlayDeathSequence());
    }

    public void ResetAlive()
    {
        isDead = false;
        StopAllCoroutines();
        deathVisual.SetActive(false);
        aliveVisual.SetActive(true);
        playerAnimator.EnableAnimator();
    }

    private IEnumerator PlayDeathSequence()
    {
        // Copy v? trí và h??ng nhìn c?a nhân v?t s?ng sang DeadCat
        deathVisual.transform.position = aliveVisual.transform.position;
        deathVisual.transform.localScale = aliveVisual.transform.localScale;

        deathVisual.SetActive(true);

        yield return null;
        yield return null;

        float clipLength = 1f;
        AnimatorClipInfo[] clips = deathAnimator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
            clipLength = clips[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        OnDeathAnimationComplete.Invoke();
    }
}