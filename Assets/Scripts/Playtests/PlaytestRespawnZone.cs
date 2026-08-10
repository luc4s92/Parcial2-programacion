using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class PlaytestRespawnZone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    public void Configure(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D playerBody = other.attachedRigidbody;

        if (playerBody == null || !playerBody.CompareTag("Player") || respawnPoint == null)
            return;

        playerBody.position = respawnPoint.position;
        playerBody.linearVelocity = Vector2.zero;
        Physics2D.SyncTransforms();
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D trigger = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(0.85f, 0.2f, 0.2f, 0.25f);
        Gizmos.DrawCube(transform.position + (Vector3)trigger.offset, trigger.size);
    }
}
