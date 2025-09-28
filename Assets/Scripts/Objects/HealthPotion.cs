using UnityEngine;

public class HealthPotion : Item
{
    public override void Use(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            if (health.Life < health.MaxLife) //  solo cura si no está full
            {
                health.Heal(1);
                Debug.Log("Agarraste una poción -> +1 vida");
            }
            else
            {
                Debug.Log("La vida está al máximo, no puedes curarte más.");
            }
        }

        Destroy(gameObject); // Siempre desaparece al recogerlo
    }
}
