using UnityEngine;

public sealed class ShurikenChargePickup : Item
{
    [Min(1)]
    [SerializeField] private int charges = 1;

    public override bool TryUse(GameObject player)
    {
        return PlayerProgression.Shuriken.RestoreCharges(charges) > 0;
    }
}
