using UnityEngine;

[System.Serializable]
public class Level3FlappyParallaxLayer
{
    [Header("Layer Sprite")]
    public SpriteRenderer sprite;

    [Header("Parallax Speed")]
    [Range(0f, 20f)]
    public float speed = 1f;

    // Runtime
    [HideInInspector] public Transform left;
    [HideInInspector] public Transform middle;
    [HideInInspector] public Transform right;

    [HideInInspector] public float width;
}

public class Level3FlappyParallax : MonoBehaviour
{
    [Header("All Parallax Layers")]
    [SerializeField]
    private Level3FlappyParallaxLayer[] layers;

    private void Start()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            SetupLayer(layers[i]);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < layers.Length; i++)
        {
            MoveLayer(layers[i], dt);
        }
    }

    private void SetupLayer(Level3FlappyParallaxLayer layer)
    {
        if (layer.sprite == null)
            return;

        // object gốc
        layer.middle = layer.sprite.transform;

        // width sprite
        layer.width =
            layer.sprite.bounds.size.x;

        // clone trái
        GameObject leftObj = Instantiate(
            layer.sprite.gameObject,
            layer.middle.position +
            Vector3.left * layer.width,
            Quaternion.identity,
            transform
        );

        // clone phải
        GameObject rightObj = Instantiate(
            layer.sprite.gameObject,
            layer.middle.position +
            Vector3.right * layer.width,
            Quaternion.identity,
            transform
        );

        leftObj.name = layer.sprite.gameObject.name + "_Left";
        rightObj.name = layer.sprite.gameObject.name + "_Right";

        layer.left = leftObj.transform;
        layer.right = rightObj.transform;
    }

    private void MoveLayer(
        Level3FlappyParallaxLayer layer,
        float dt)
    {
        float moveAmount =
            layer.speed * dt;

        Vector3 move =
            Vector3.left * moveAmount;

        layer.left.position += move;
        layer.middle.position += move;
        layer.right.position += move;

        // recycle tile trái
        if (layer.left.position.x <= -layer.width)
        {
            RecycleLeft(layer);
        }
    }

    private void RecycleLeft(
        Level3FlappyParallaxLayer layer)
    {
        layer.left.position =
            new Vector3(
                layer.right.position.x + layer.width,
                layer.left.position.y,
                layer.left.position.z
            );

        // rotate references
        Transform oldLeft = layer.left;

        layer.left = layer.middle;
        layer.middle = layer.right;
        layer.right = oldLeft;
    }
}