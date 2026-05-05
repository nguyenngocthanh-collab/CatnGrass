using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public SpriteRenderer sprite;   // kéo forest_sky_0, forest_mountain_0... vào ?ây
    [Range(0f, 1f)] public float speed;

    // Runtime
    [HideInInspector] public List<Transform> tiles = new List<Transform>();
    [HideInInspector] public float tileWidth;
}

public class Paralax001 : MonoBehaviour
{
    public Transform cam;
    public ParallaxLayer[] layers;
    private float camPrevX;

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;
        camPrevX = cam.position.x;

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

        // T?o 3 tile: original ? gi?a, clone 1 bên trái, 1 bên ph?i
        GameObject left = Instantiate(original.gameObject);
        GameObject right = Instantiate(original.gameObject);

        left.name = original.name + "_L";
        right.name = original.name + "_R";

        // Gi? nguyên Y, Z, sorting layer
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

        // Copy sorting order
        SpriteRenderer srL = left.GetComponent<SpriteRenderer>();
        SpriteRenderer srR = right.GetComponent<SpriteRenderer>();
        if (srL != null) { srL.sortingLayerName = layer.sprite.sortingLayerName; srL.sortingOrder = layer.sprite.sortingOrder; }
        if (srR != null) { srR.sortingLayerName = layer.sprite.sortingLayerName; srR.sortingOrder = layer.sprite.sortingOrder; }

        layer.tiles.Clear();
        layer.tiles.Add(left.transform);
        layer.tiles.Add(original);
        layer.tiles.Add(right.transform);
    }

    void LateUpdate()
    {
        float deltaX = cam.position.x - camPrevX;

        foreach (var layer in layers)
        {
            if (layer.tiles.Count == 0) continue;

            foreach (var tile in layer.tiles)
                tile.position += new Vector3(deltaX * layer.speed, 0f, 0f);

            LoopLayer(layer);
        }

        camPrevX = cam.position.x;
    }

    void LoopLayer(ParallaxLayer layer)
    {
        float camX = cam.position.x;
        float w = layer.tileWidth;

        foreach (var tile in layer.tiles)
        {
            if (tile.position.x + w / 2f < camX - w)
            {
                Transform rightmost = GetExtreme(layer.tiles, true);
                tile.position = new Vector3(
                    rightmost.position.x + w,
                    tile.position.y,
                    tile.position.z
                );
            }
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
            if (rightmost && t.position.x > result.position.x) result = t;
            if (!rightmost && t.position.x < result.position.x) result = t;
        }
        return result;
    }
}