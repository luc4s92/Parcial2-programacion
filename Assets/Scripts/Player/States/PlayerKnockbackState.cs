using System;
using UnityEngine;

internal sealed class PlayerKnockbackState : IState
{
    private readonly PlayerDamageReaction damageReaction;
    private readonly float duration;
    private readonly Action resolveLocomotion;
    private Vector2 direction;
    private float elapsedTime;

    internal PlayerKnockbackState(
        PlayerDamageReaction damageReaction,
        float duration,
        Action resolveLocomotion)
    {
        this.damageReaction = damageReaction;
        this.duration = duration;
        this.resolveLocomotion = resolveLocomotion;
    }

    internal void Configure(Vector2 direction)
    {
        this.direction = direction;
    }

    void IState.Enter()
    {
        elapsedTime = 0f;
        damageReaction.BeginKnockback(direction);
    }

    void IState.Tick()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
            resolveLocomotion();
    }

    void IState.Exit()
    {
        damageReaction.EndKnockback();
    }
}
