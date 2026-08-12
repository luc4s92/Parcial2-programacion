using System;

public interface IShurikenInventory
{
    event Action Changed;

    bool IsUnlocked { get; }
    int CurrentCharges { get; }
    int MaxCharges { get; }
    bool CanThrow { get; }

    bool Unlock();
    bool TryConsumeCharge();
    int RestoreCharges(int amount);
}
