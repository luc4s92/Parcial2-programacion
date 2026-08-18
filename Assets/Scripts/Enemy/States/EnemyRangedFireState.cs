using System;
using UnityEngine;

internal sealed class EnemyRangedFireState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyRangedCombat combat;
    private readonly Action resolveAwareness;
    private readonly float recoveryDuration;

    private float elapsedTime;

    internal EnemyRangedFireState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyRangedCombat combat,
        float recoveryDuration,
        Action resolveAwareness)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.combat = combat;
        this.recoveryDuration = Mathf.Max(0f, recoveryDuration);
        this.resolveAwareness = resolveAwareness;
    }

    void IState.Enter()
    {
        elapsedTime = 0f;
        movement.StopCompletely();

        if (!targeting.HasTarget)
            return;

        movement.Face(targeting.Target);
        combat.TryFire(targeting.Target);
    }

    void IState.Tick()
    {
        movement.StopCompletely();
        elapsedTime += Time.fixedDeltaTime;

        if (elapsedTime >= recoveryDuration)
            resolveAwareness();
    }

    void IState.Exit()
    {
    }
}
