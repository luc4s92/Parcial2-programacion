using System;
using UnityEngine;

internal sealed class EnemyHitState : IState
{
    private readonly EnemyDamageReaction damageReaction;
    private readonly float recoveryDuration;
    private readonly Action resolveAwareness;
    private Vector2 direction;
    private bool hitFinished;
    private float elapsedTime;

    internal EnemyHitState(
        EnemyDamageReaction damageReaction,
        float recoveryDuration,
        Action resolveAwareness)
    {
        this.damageReaction = damageReaction;
        this.recoveryDuration = recoveryDuration;
        this.resolveAwareness = resolveAwareness;
    }

    internal void Configure(Vector2 direction)
    {
        this.direction = direction;
    }

    internal void CompleteHit()
    {
        hitFinished = true;
    }

    void IState.Enter()
    {
        hitFinished = false;
        elapsedTime = 0f;
        damageReaction.BeginHit(direction);
    }

    void IState.Tick()
    {
        elapsedTime += Time.fixedDeltaTime;

        if (hitFinished || elapsedTime >= recoveryDuration)
            resolveAwareness();
    }

    void IState.Exit()
    {
        damageReaction.EndHit();
    }
}
