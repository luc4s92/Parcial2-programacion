using System;
using UnityEngine;

internal sealed class EnemyAttackWindupState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAttackTelegraph telegraph;
    private readonly Func<bool> canContinue;
    private readonly Action requestCancelled;
    private readonly Action requestAttack;
    private readonly float duration;

    private float elapsedTime;

    internal EnemyAttackWindupState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAttackTelegraph telegraph,
        float duration,
        Func<bool> canContinue,
        Action requestCancelled,
        Action requestAttack)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.telegraph = telegraph;
        this.duration = Mathf.Max(0f, duration);
        this.canContinue = canContinue;
        this.requestCancelled = requestCancelled;
        this.requestAttack = requestAttack;
    }

    void IState.Enter()
    {
        elapsedTime = 0f;
        movement.Stop();

        if (targeting.HasTarget)
            movement.Face(targeting.Target);

        telegraph.Begin();
    }

    void IState.Tick()
    {
        movement.Stop();

        if (!targeting.HasTarget || !canContinue())
        {
            requestCancelled();
            return;
        }

        movement.Face(targeting.Target);
        elapsedTime += Time.fixedDeltaTime;
        telegraph.Tick(Time.fixedDeltaTime);

        if (elapsedTime >= duration)
            requestAttack();
    }

    void IState.Exit()
    {
        telegraph.End();
    }
}
