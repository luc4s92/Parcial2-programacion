using UnityEngine;

public class HealthPotion : Item
{
    public override void Use(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            if (health.Life < health.MaxLife) //  solo cura si no est� full
            {
                health.Heal(1);
                Debug.Log("Agarraste una poci�n -> +1 vida");
            }
            else
            {
                Debug.Log("La vida est� al m�ximo, no puedes curarte m�s.");
            }
        }

        Destroy(gameObject); // Siempre desaparece al recogerlo
    }
}
