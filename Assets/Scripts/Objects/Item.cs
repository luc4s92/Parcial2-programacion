using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private ItemAudio itemAudio;
    public abstract void Use(GameObject player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Use(other.gameObject);
            itemAudio.PlayPickUp();
        }
    }



  
}
