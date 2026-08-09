using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class HellHoundSpawnTrigger : MonoBehaviour
{
    [SerializeField] private HellHoundEnemy hellHoundPrefab;
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] private float spawnYOffset;
    [SerializeField] private bool triggerOnce = true;

    private bool wasTriggered;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((triggerOnce && wasTriggered) || hellHoundPrefab == null)
            return;

        Rigidbody2D playerBody = other.attachedRigidbody;
        GameObject playerObject = playerBody != null
            ? playerBody.gameObject
            : other.gameObject;

        if (!playerObject.CompareTag("Player"))
            return;

        float playerDirection = ResolvePlayerDirection(playerObject.transform, playerBody);
        Vector3 spawnPosition = new Vector3(
            playerObject.transform.position.x + playerDirection * Mathf.Abs(spawnDistance),
            transform.position.y + spawnYOffset,
            transform.position.z
        );

        HellHoundEnemy hellHound = Instantiate(
            hellHoundPrefab,
            spawnPosition,
            Quaternion.identity
        );
        hellHound.ConfigureRunDirection(-playerDirection);
        wasTriggered = true;
    }

    private static float ResolvePlayerDirection(
        Transform playerTransform,
        Rigidbody2D playerBody)
    {
        if (playerBody != null && Mathf.Abs(playerBody.linearVelocity.x) > 0.1f)
            return Mathf.Sign(playerBody.linearVelocity.x);

        return playerTransform.localScale.x < 0f ? -1f : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.right * Mathf.Abs(spawnDistance)
        );
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.left * Mathf.Abs(spawnDistance)
        );
    }
}
