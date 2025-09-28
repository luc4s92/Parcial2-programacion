using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract void Use(GameObject player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Use(other.gameObject);
        }
    }
}
