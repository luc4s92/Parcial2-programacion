using UnityEngine;

public class HealthPotion : Item
{
    public override bool TryUse(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health == null || health.Life >= health.MaxLife)
        {
            Debug.Log("La vida esta al maximo, no puedes curarte mas.");
            return false;
        }

        health.Heal(1);
        Debug.Log("Agarraste una pocion -> +1 vida");
        return true;
    }
}
