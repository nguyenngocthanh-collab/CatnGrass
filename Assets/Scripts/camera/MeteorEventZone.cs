using UnityEngine;
using System.Collections;

public class MeteorEventZone : MonoBehaviour
{
    [Header("References")]
    public CameraFollowv2 cameraFollow;
    public Camera mainCamera;

    [Header("Player")]
    public GameObject player;

    [Tooltip("Rigidbody2D của player")]
    public Rigidbody2D playerRb;

    [Tooltip("Kéo tất cả script điều khiển player vào")]
    public MonoBehaviour[] playerControlScripts;

    [Header("Player Freeze")]
    [Tooltip("Bật/tắt khóa player")]
    public bool freezePlayer = true;

    [Header("Meteor")]
    public Transform meteor;

    public Transform meteorStartPoint;
    public Transform meteorEndPoint;

    [Range(1f, 100f)]
    public float meteorSpeed = 20f;

    [Header("Camera Follow")]
    [Tooltip("Bật/tắt camera track meteor")]
    public bool enableCameraTrack = true;

    [Tooltip("Camera track meteor bao lâu")]
    public float cameraTrackDuration = 1.5f;

    [Range(1f, 20f)]
    public float cameraMoveSpeed = 5f;

    [Header("Camera Shake")]
    [Tooltip("Bật/tắt rung camera")]
    public bool enableCameraShake = true;

    [Tooltip("Độ mạnh rung")]
    public float shakeStrength = 0.15f;

    [Tooltip("Tốc độ rung")]
    public float shakeSpeed = 35f;

    [Tooltip("Rung bao lâu")]
    public float shakeDuration = 1f;

    [Header("Event")]
    [Tooltip("Tổng thời gian event")]
    public float eventDuration = 3f;

    public bool playOnlyOnce = true;

    private bool played = false;

    private Vector3 originalCamPos;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (played && playOnlyOnce)
            return;

        if (other.CompareTag("Player"))
        {
            played = true;
            StartCoroutine(EventSequence());
        }
    }

    IEnumerator EventSequence()
    {
        // =========================
        // LOCK PLAYER
        // =========================

        if (freezePlayer)
        {
            foreach (MonoBehaviour script in playerControlScripts)
            {
                if (script != null)
                    script.enabled = false;
            }

            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.simulated = false;
            }
        }

        // =========================
        // CAMERA FOLLOW
        // =========================

        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        // =========================
        // CAMERA SHAKE
        // =========================

        if (enableCameraShake)
        {
            StartCoroutine(CameraShake());
        }

        // =========================
        // RESET METEOR
        // =========================

        meteor.position = meteorStartPoint.position;

        // Force smoke play ngay lập tức
        ParticleSystem ps =
            meteor.GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            ps.Clear(true);
            ps.Play(true);
        }

        float timer = 0f;

        while (timer < eventDuration)
        {
            timer += Time.deltaTime;

            // =========================
            // METEOR MOVE
            // =========================

            meteor.position = Vector3.MoveTowards(
                meteor.position,
                meteorEndPoint.position,
                meteorSpeed * Time.deltaTime
            );

            // =========================
            // CAMERA TRACK
            // =========================

            if (enableCameraTrack &&
                timer <= cameraTrackDuration)
            {
                Vector3 camTarget = new Vector3(
                    meteor.position.x,
                    meteor.position.y,
                    mainCamera.transform.position.z
                );

                mainCamera.transform.position = Vector3.Lerp(
                    mainCamera.transform.position,
                    camTarget,
                    cameraMoveSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        // =========================
        // END EVENT
        // =========================

        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }

        if (freezePlayer)
        {
            if (playerRb != null)
            {
                playerRb.simulated = true;
            }

            foreach (MonoBehaviour script in playerControlScripts)
            {
                if (script != null)
                    script.enabled = true;
            }
        }
    }

    IEnumerator CameraShake()
    {
        originalCamPos = mainCamera.transform.position;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            float x =
                Mathf.PerlinNoise(
                    Time.time * shakeSpeed,
                    0f
                ) - 0.5f;

            float y =
                Mathf.PerlinNoise(
                    0f,
                    Time.time * shakeSpeed
                ) - 0.5f;

            Vector3 shakeOffset =
                new Vector3(x, y, 0f) * shakeStrength;

            mainCamera.transform.position += shakeOffset;

            yield return null;
        }

        mainCamera.transform.position = originalCamPos;
    }
}