using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public SpriteRenderer sprite;

    [Range(0f, 1f)]
    public float speed;

    [Header("Follow Camera Y")]
    public bool followY = false;

    // Runtime
    [HideInInspector] public List<Transform> tiles = new List<Transform>();
    [HideInInspector] public float tileWidth;
}

public class Paralax001 : MonoBehaviour
{
    public Transform cam;
    public ParallaxLayer[] layers;

    private float camPrevX;
    private float camPrevY;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        camPrevX = cam.position.x;
        camPrevY = cam.position.y;

        foreach (var layer in layers)
        {
            if (layer.sprite == null) continue;

            SetupLayer(layer);
        }
    }

    void SetupLayer(ParallaxLayer layer)
    {
        Transform original = layer.sprite.transform;

        layer.tileWidth = layer.sprite.bounds.size.x;

        // Tạo tile trái/phải
        GameObject left = Instantiate(original.gameObject);
        GameObject right = Instantiate(original.gameObject);

        left.name = original.name + "_L";
        right.name = original.name + "_R";

        left.transform.position = new Vector3(
            original.position.x - layer.tileWidth,
            original.position.y,
            original.position.z
        );

        right.transform.position = new Vector3(
            original.position.x + layer.tileWidth,
            original.position.y,
            original.position.z
        );

        // Copy sorting
        SpriteRenderer srL = left.GetComponent<SpriteRenderer>();
        SpriteRenderer srR = right.GetComponent<SpriteRenderer>();

        if (srL != null)
        {
            srL.sortingLayerName = layer.sprite.sortingLayerName;
            srL.sortingOrder = layer.sprite.sortingOrder;
        }

        if (srR != null)
        {
            srR.sortingLayerName = layer.sprite.sortingLayerName;
            srR.sortingOrder = layer.sprite.sortingOrder;
        }

        layer.tiles.Clear();

        layer.tiles.Add(left.transform);
        layer.tiles.Add(original);
        layer.tiles.Add(right.transform);
    }

    void LateUpdate()
    {
        float deltaX = cam.position.x - camPrevX;
        float deltaY = cam.position.y - camPrevY;

        foreach (var layer in layers)
        {
            if (layer.tiles.Count == 0) continue;

            foreach (var tile in layer.tiles)
            {
                float moveX = deltaX * layer.speed;

                // Nếu bật followY thì layer sẽ follow camera/player theo Y
                float moveY = layer.followY ? deltaY : 0f;

                tile.position += new Vector3(moveX, moveY, 0f);
            }

            LoopLayer(layer);
        }

        camPrevX = cam.position.x;
        camPrevY = cam.position.y;
    }

    void LoopLayer(ParallaxLayer layer)
    {
        float camX = cam.position.x;
        float w = layer.tileWidth;

        foreach (var tile in layer.tiles)
        {
            // Quá trái
            if (tile.position.x + w / 2f < camX - w)
            {
                Transform rightmost = GetExtreme(layer.tiles, true);

                tile.position = new Vector3(
                    rightmost.position.x + w,
                    tile.position.y,
                    tile.position.z
                );
            }

            // Quá phải
            else if (tile.position.x - w / 2f > camX + w)
            {
                Transform leftmost = GetExtreme(layer.tiles, false);

                tile.position = new Vector3(
                    leftmost.position.x - w,
                    tile.position.y,
                    tile.position.z
                );
            }
        }
    }

    Transform GetExtreme(List<Transform> tiles, bool rightmost)
    {
        Transform result = tiles[0];

        foreach (var t in tiles)
        {
            if (rightmost && t.position.x > result.position.x)
                result = t;

            if (!rightmost && t.position.x < result.position.x)
                result = t;
        }

        return result;
    }
}