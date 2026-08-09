using UnityEngine;

internal sealed class EnemyRun
{
    private readonly EnemyMovement movement;
    private float direction;

    internal EnemyRun(EnemyMovement movement, float initialDirection)
    {
        this.movement = movement;
        SetDirection(initialDirection);
    }

    internal void SetDirection(float value)
    {
        direction = value < 0f ? -1f : 1f;
    }

    internal void Tick()
    {
        movement.FaceDirection(direction);
        movement.Move(direction);
    }
}
