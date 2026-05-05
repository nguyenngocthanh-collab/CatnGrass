using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private playermovetest2 movement;
    [SerializeField] private Animator aliveAnimator;
    [SerializeField] private Transform visualTransform;

    [Header("Thresholds")]
    [SerializeField] private float runThreshold = 0.1f;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsGrounded = Animator.StringToHash("IsGrounded");

    private Rigidbody2D rb;
    private bool isDisabled = false;
    private Vector3 originalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (movement == null)
            movement = GetComponent<playermovetest2>();
        originalScale = visualTransform.localScale;
    }

    void Update()
    {
        if (isDisabled) return;
        UpdateAnimatorParams();
        UpdateFlip();
    }

    public void DisableAnimator()
    {
        isDisabled = true;
        aliveAnimator.enabled = false;
    }

    public void EnableAnimator()
    {
        isDisabled = false;
        aliveAnimator.enabled = true;
    }

    private void UpdateAnimatorParams()
    {
        float inputX = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        bool grounded = movement.IsGrounded;

        aliveAnimator.SetFloat(HashSpeed, inputX);
        aliveAnimator.SetBool(HashIsGrounded, grounded);
    }

    private void UpdateFlip()
    {
        float vx = rb.linearVelocity.x;
        if (vx > runThreshold)
            visualTransform.localScale = originalScale;
        else if (vx < -runThreshold)
            visualTransform.localScale = new Vector3(
                -originalScale.x, originalScale.y, originalScale.z);
    }
}