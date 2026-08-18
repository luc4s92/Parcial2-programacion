using System;

internal sealed class EnemyAttackState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly EnemyCombat combat;
    private readonly Action resolveAwareness;
    private readonly float maxDuration;
    private bool attackFinished;
    private float elapsedTime;

    internal EnemyAttackState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        EnemyCombat combat,
        float maxDuration,
        Action resolveAwareness)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.combat = combat;
        this.maxDuration = Math.Max(0.05f, maxDuration);
        this.resolveAwareness = resolveAwareness;
    }

    internal void CompleteAttack()
    {
        attackFinished = true;
    }

    void IState.Enter()
    {
        attackFinished = false;
        elapsedTime = 0f;
        movement.Stop();

        if (targeting.HasTarget)
            movement.Face(targeting.Target);

        combat.BeginAttack();
        animationController.SetAttacking(true);
    }

    void IState.Tick()
    {
        movement.Stop();
        elapsedTime += UnityEngine.Time.fixedDeltaTime;

        if (attackFinished || elapsedTime >= maxDuration)
            resolveAwareness();
    }

    void IState.Exit()
    {
        animationController.SetAttacking(false);
        combat.EndAttack();
    }
}
