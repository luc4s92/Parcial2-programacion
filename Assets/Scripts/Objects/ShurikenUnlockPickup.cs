using UnityEngine;

public sealed class ShurikenUnlockPickup : Item
{
    public override bool TryUse(GameObject player)
    {
        return PlayerProgression.Shuriken.Unlock();
    }
}
