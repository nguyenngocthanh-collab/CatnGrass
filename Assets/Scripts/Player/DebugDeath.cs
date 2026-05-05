using UnityEngine;

public class DebugDeath : MonoBehaviour
{
    private PlayerDead death;

    void Awake() => death = GetComponent<PlayerDead>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            death.TriggerDeath();

        if (Input.GetKeyDown(KeyCode.L))
            death.ResetAlive();
    }
}