using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private ItemAudio itemAudio;

    private bool isConsumed;

    public abstract bool TryUse(GameObject player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConsumed ||
            !other.CompareTag("Player") ||
            !TryUse(other.gameObject))
        {
            return;
        }

        isConsumed = true;
        itemAudio?.PlayPickUp();
        Destroy(gameObject);
    }
}
