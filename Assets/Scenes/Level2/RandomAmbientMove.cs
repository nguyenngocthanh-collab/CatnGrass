using UnityEngine;

public class RandomAmbientMove : MonoBehaviour
{
    [SerializeField]
    private float destroyDistance = 40f;

    private Vector2 direction;

    private float speed;

    private Vector3 startPos;

    public void SetMoveData(
        Vector2 dir,
        float moveSpeed)
    {
        direction = dir;

        speed = moveSpeed;
    }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position +=
            (Vector3)direction *
            speed *
            Time.deltaTime;

        if (Vector3.Distance(
            startPos,
            transform.position)
            >= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}