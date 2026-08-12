using System;

public sealed class ShurikenInventory : IShurikenInventory
{
    public event Action Changed;

    public ShurikenInventory(int maxCharges)
    {
        MaxCharges = Math.Max(1, maxCharges);
    }

    public bool IsUnlocked { get; private set; }
    public int CurrentCharges { get; private set; }
    public int MaxCharges { get; }
    public bool CanThrow => IsUnlocked && CurrentCharges > 0;

    public bool Unlock()
    {
        if (IsUnlocked)
            return false;

        IsUnlocked = true;
        CurrentCharges = MaxCharges;
        Changed?.Invoke();
        return true;
    }

    public bool TryConsumeCharge()
    {
        if (!CanThrow)
            return false;

        CurrentCharges--;
        Changed?.Invoke();
        return true;
    }

    public int RestoreCharges(int amount)
    {
        if (!IsUnlocked || amount <= 0 || CurrentCharges >= MaxCharges)
            return 0;

        int previousCharges = CurrentCharges;
        CurrentCharges = Math.Min(CurrentCharges + amount, MaxCharges);
        Changed?.Invoke();
        return CurrentCharges - previousCharges;
    }

    internal void Reset()
    {
        IsUnlocked = false;
        CurrentCharges = 0;
        Changed?.Invoke();
    }
}
