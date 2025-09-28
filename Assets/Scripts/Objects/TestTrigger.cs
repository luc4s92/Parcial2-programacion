using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[TestTrigger] Colisión con: {collision.gameObject.name}, Tag: {collision.tag}");
    }
}
