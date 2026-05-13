using UnityEngine;
using System.Collections;

public class GhostEventManager : MonoBehaviour
{
    [System.Serializable]
    public class GhostEvent
    {
        [Header("Event Time")]

        public float startTime = 10f;

        public float duration = 5f;
    }

    [Header("Events")]

    [SerializeField]
    private GhostEvent[] events;

    [Header("Recovery")]

    [SerializeField]
    private float recoveryDuration = 3f;

    // ghost đang spawn
    public bool IsGhostEventActive
    {
        get;
        private set;
    }

    // pipe bị khoá
    public bool BlockPipeSpawn
    {
        get;
        private set;
    }

    private float timer;

    private int currentEventIndex;

    private bool eventStarted;

    private void Update()
    {
        if (Level3FlappyGameManager.Instance == null)
            return;

        if (!Level3FlappyGameManager.Instance.HasStarted)
            return;

        if (Level3FlappyGameManager.Instance.IsDead)
            return;

        if (currentEventIndex >= events.Length)
            return;

        timer += Time.deltaTime;

        GhostEvent currentEvent =
            events[currentEventIndex];

        // =========================
        // START EVENT
        // =========================

        if (!eventStarted &&
            timer >= currentEvent.startTime)
        {
            eventStarted = true;

            IsGhostEventActive = true;

            // khoá pipe
            BlockPipeSpawn = true;
        }

        // =========================
        // END EVENT
        // =========================

        if (eventStarted &&
            timer >=
            currentEvent.startTime +
            currentEvent.duration)
        {
            eventStarted = false;

            // ghost NGỪNG spawn NGAY
            IsGhostEventActive = false;

            // bắt đầu recovery
            StartCoroutine(
                RecoveryPhase()
            );

            currentEventIndex++;
        }
    }

    private IEnumerator RecoveryPhase()
    {
        // chờ ghost cũ bay ra ngoài
        yield return new WaitForSeconds(
            recoveryDuration
        );

        // mở lại pipe
        BlockPipeSpawn = false;
    }
}