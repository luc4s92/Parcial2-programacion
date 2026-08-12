using UnityEngine;

public static class PlayerProgression
{
    private const int InitialShurikenCapacity = 3;

    private static ShurikenInventory shurikenInventory;

    public static IShurikenInventory Shuriken => GetShurikenInventory();

    public static void ResetForNewGame()
    {
        GetShurikenInventory().Reset();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeSession()
    {
        shurikenInventory = new ShurikenInventory(InitialShurikenCapacity);
    }

    private static ShurikenInventory GetShurikenInventory()
    {
        shurikenInventory ??= new ShurikenInventory(InitialShurikenCapacity);
        return shurikenInventory;
    }
}
