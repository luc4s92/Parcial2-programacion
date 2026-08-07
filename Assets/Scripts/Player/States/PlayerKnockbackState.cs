using System;
using UnityEngine;

internal sealed class PlayerKnockbackState : IPlayerState
{
    private readonly PlayerDamageReaction damageReaction;
    private readonly float duration;
    private readonly Action requestLocomotion;
    private Vector2 direction;
    private Collider2D enemyCollider;
    private float elapsedTime;

    internal PlayerKnockbackState(
        PlayerDamageReaction damageReaction,
        float duration,
        Action requestLocomotion)
    {
        this.damageReaction = damageReaction;
        this.duration = duration;
        this.requestLocomotion = requestLocomotion;
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
            requestLocomotion();
    }

    void IPlayerState.Exit()
    {
        damageReaction.EndKnockback(enemyCollider);
        enemyCollider = null;
    }
}
