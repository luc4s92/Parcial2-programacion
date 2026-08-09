using System;
using UnityEngine;

internal sealed class PlayerKnockbackState : IPlayerState
{
    private readonly PlayerDamageReaction damageReaction;
    private readonly float duration;
    private readonly Action resolveLocomotion;
    private Vector2 direction;
    private Collider2D enemyCollider;
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

    internal void Configure(Vector2 direction, Collider2D enemyCollider)
    {
        this.direction = direction;
        this.enemyCollider = enemyCollider;
    }

    void IPlayerState.Enter()
    {
        elapsedTime = 0f;
        damageReaction.BeginKnockback(direction, enemyCollider);
    }

    void IPlayerState.Tick()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
            resolveLocomotion();
    }

    void IPlayerState.Exit()
    {
        damageReaction.EndKnockback(enemyCollider);
        enemyCollider = null;
    }
}
