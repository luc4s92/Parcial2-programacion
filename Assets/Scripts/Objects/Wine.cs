using UnityEngine;

public class Vino : Item
{
    public override bool TryUse(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement == null)
            return false;

        Debug.Log("Tomaste un vino -> estas ebrio, pierdes velocidad!");
        movement.ApplySpeedModifier(0.5f, 5f);
        return true;
    }
}
