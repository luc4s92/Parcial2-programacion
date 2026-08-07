using UnityEngine;

internal sealed class PlayerSpeedModifier
{
    private readonly float baseSpeed;
    private float remainingDuration;

    internal float CurrentSpeed { get; private set; }

    internal PlayerSpeedModifier(float baseSpeed)
    {
        this.baseSpeed = baseSpeed;
        CurrentSpeed = baseSpeed;
    }

    internal void Apply(float multiplier, float duration)
    {
        if (remainingDuration > 0f)
        {
            remainingDuration += duration;
            Debug.Log($"[SpeedModifier] Se extendio duracion, tiempo restante: {remainingDuration:F2}s");
            return;
        }

        remainingDuration = duration;
        CurrentSpeed = baseSpeed * multiplier;
        Debug.Log($"[SpeedModifier] Velocidad modificada: {CurrentSpeed} (x{multiplier}) durante {duration}s");
    }

    internal void Tick(float deltaTime)
    {
        if (remainingDuration <= 0f) return;

        remainingDuration -= deltaTime;
        if (remainingDuration > 0f) return;

        remainingDuration = 0f;
        CurrentSpeed = baseSpeed;
        Debug.Log($"[SpeedModifier] Efecto terminado -> Velocidad restaurada a {CurrentSpeed}");
    }
}
