using UnityEngine;
using System.Collections;

public class MeteorCinematicEvent : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    public Rigidbody2D playerRb;

    public Camera mainCam;

    public GameObject meteorPrefab;

    [Header("Meteor Spawn")]
    [Tooltip("Spawn relative player")]
    public Vector2 spawnOffset = new Vector2(14f, 10f);

    [Tooltip("Meteor bay tới player")]
    public bool autoAimPlayer = true;

    public Vector2 customDirection = new Vector2(-1f, -0.5f);

    [Tooltip("Tốc độ meteor")]
    public float meteorSpeed = 40f;

    [Tooltip("Scale meteor")]
    public float meteorSize = 6f;

    [Tooltip("Xoay thêm")]
    public float extraAngle = -90f;

    [Tooltip("Tự hủy")]
    public float meteorLifetime = 8f;

    [Header("Freeze")]
    public float freezeDuration = 2f;

    [Header("Camera Cinematic")]
    [Tooltip("Camera lia về phía meteor")]
    public Vector2 cameraShift = new Vector2(3f, 2f);

    [Tooltip("Tốc độ lia")]
    public float cameraMoveSpeed = 4f;

    [Tooltip("Giữ camera")]
    public float cameraHoldTime = 0.5f;

    [Header("Trigger")]
    public bool triggerOnce = true;

    private bool triggered;

    private Vector3 camOriginalPos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject root =
            other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (triggered && triggerOnce)
            return;

        triggered = true;

        Debug.Log("[MeteorEvent] START");

        StartCoroutine(EventRoutine());
    }

    IEnumerator EventRoutine()
    {
        // Freeze player
        FreezePlayer(true);

        // Spawn meteor
        GameObject meteor = SpawnMeteor();

        if (meteor == null)
            yield break;

        // Direction
        Vector2 dir;

        if (autoAimPlayer)
        {
            dir =
                (
                    player.position -
                    meteor.transform.position
                ).normalized;
        }
        else
        {
            dir =
                customDirection.normalized;
        }

        // Camera cinematic
        yield return StartCoroutine(
            CameraCinematic(dir)
        );

        // Chờ
        yield return new WaitForSeconds(
            freezeDuration
        );

        // Unfreeze
        FreezePlayer(false);

        Debug.Log("[MeteorEvent] END");
    }

    GameObject SpawnMeteor()
    {
        if (
            meteorPrefab == null ||
            player == null
        )
        {
            Debug.LogError(
                "[MeteorEvent] Missing Ref"
            );

            return null;
        }

        Vector3 spawnPos =
            player.position +
            (Vector3)spawnOffset;

        GameObject meteor = Instantiate(
            meteorPrefab,
            spawnPos,
            Quaternion.identity
        );

        // Scale
        meteor.transform.localScale =
            Vector3.one * meteorSize;

        // Direction
        Vector2 dir;

        if (autoAimPlayer)
        {
            dir =
                (
                    player.position -
                    spawnPos
                ).normalized;
        }
        else
        {
            dir =
                customDirection.normalized;
        }

        // Rotation
        float angle =
            Mathf.Atan2(
                dir.y,
                dir.x
            ) * Mathf.Rad2Deg;

        meteor.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle + extraAngle
            );

        // Rigidbody
        Rigidbody2D rb =
            meteor.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;

            rb.linearVelocity =
                dir * meteorSpeed;
        }

        Destroy(
            meteor,
            meteorLifetime
        );

        return meteor;
    }

    IEnumerator CameraCinematic(Vector2 dir)
    {
        if (mainCam == null)
            yield break;

        camOriginalPos =
            mainCam.transform.position;

        Vector3 targetPos =
            camOriginalPos +
            new Vector3(
                dir.x * cameraShift.x,
                dir.y * cameraShift.y,
                0f
            );

        float t = 0f;

        // Lia tới meteor
        while (t < 1f)
        {
            t +=
                Time.deltaTime *
                cameraMoveSpeed;

            mainCam.transform.position =
                Vector3.Lerp(
                    camOriginalPos,
                    targetPos,
                    t
                );

            yield return null;
        }

        // Giữ
        yield return new WaitForSeconds(
            cameraHoldTime
        );

        t = 0f;

        // Trả camera
        while (t < 1f)
        {
            t +=
                Time.deltaTime *
                cameraMoveSpeed;

            mainCam.transform.position =
                Vector3.Lerp(
                    targetPos,
                    camOriginalPos,
                    t
                );

            yield return null;
        }
    }

    void FreezePlayer(bool freeze)
    {
        if (playerRb == null)
            return;

        if (freeze)
        {
            playerRb.linearVelocity =
                Vector2.zero;

            playerRb.constraints =
                RigidbodyConstraints2D.FreezeAll;

            Debug.Log("[MeteorEvent] FREEZE");
        }
        else
        {
            playerRb.constraints =
                RigidbodyConstraints2D.FreezeRotation;

            Debug.Log("[MeteorEvent] UNFREEZE");
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D box =
            GetComponent<BoxCollider2D>();

        if (box != null)
        {
            Gizmos.color = Color.red;

            Gizmos.matrix =
                transform.localToWorldMatrix;

            Gizmos.DrawWireCube(
                box.offset,
                box.size
            );
        }

        if (player != null)
        {
            Vector3 spawnPos =
                player.position +
                (Vector3)spawnOffset;

            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(
                spawnPos,
                0.5f
            );

            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(
                spawnPos,
                player.position
            );
        }
    }
}