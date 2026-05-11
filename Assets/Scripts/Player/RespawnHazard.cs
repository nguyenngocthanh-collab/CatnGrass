using System.Collections;
using UnityEngine;

public class RespawnHazard : MonoBehaviour
{
    [SerializeField] private bool respawnPlayer = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[RespawnHazard] Trigger Enter: " + other.name);

        if (!respawnPlayer)
        {
            Debug.Log("[RespawnHazard] respawnPlayer = false");
            return;
        }

        // tìm Player ở parent
        RespawnSystem02 respawn =
            other.GetComponentInParent<RespawnSystem02>();

        if (respawn == null)
        {
            Debug.Log("[RespawnHazard] No RespawnSystem02 Found");
            return;
        }

        Debug.Log("[RespawnHazard] PLAYER FOUND");

        StartCoroutine(RespawnNextFrame(respawn));
    }

    private IEnumerator RespawnNextFrame(RespawnSystem02 respawn)
    {
        Debug.Log("[RespawnHazard] Waiting FixedUpdate");

        yield return new WaitForFixedUpdate();

        if (respawn == null)
        {
            Debug.LogError("[RespawnHazard] RespawnSystem NULL");
            yield break;
        }

        Debug.Log("[RespawnHazard] Calling ForceRespawn");

        respawn.ForceRespawn(transform.position);
    }
}