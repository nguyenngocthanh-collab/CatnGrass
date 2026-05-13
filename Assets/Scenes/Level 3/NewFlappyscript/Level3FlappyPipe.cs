using UnityEngine;

public class Level3FlappyPipe : MonoBehaviour
{
    [Header("Destroy")]
    [SerializeField] private float destroyXPosition = -20f;

    private float moveSpeed;

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void Update()
    {
        if (Level3FlappyGameManager.Instance == null)
            return;

        if (Level3FlappyGameManager.Instance.IsDead)
            return;

        transform.position +=
            Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}