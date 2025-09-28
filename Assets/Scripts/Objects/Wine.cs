using UnityEngine;

public class Vino : Item
{
    public override void Use(GameObject player)
    {
        Debug.Log("Tomaste un vino -> estás ebrio, pierdes velocidad!");

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ApplySpeedModifier(0.5f, 5f); //  reduce a la mitad la velocidad por 5s
        }

        Destroy(gameObject);
    }
}
