using UnityEngine;

public class PowerUp : Item
{
    public override bool TryUse(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement == null)
            return false;

        Debug.Log("Agarraste un Power Up -> mas velocidad!");
        movement.ApplySpeedModifier(2f, 5f);
        return true;
    }
}
