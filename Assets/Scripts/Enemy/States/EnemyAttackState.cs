using System;

internal sealed class EnemyAttackState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly EnemyCombat combat;
    private readonly Action resolveAwareness;
    private bool attackFinished;

    internal EnemyAttackState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        EnemyCombat combat,
        Action resolveAwareness)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.combat = combat;
        this.resolveAwareness = resolveAwareness;
    }

    internal void CompleteAttack()
    {
        attackFinished = true;
    }

    void IState.Enter()
    {
        attackFinished = false;
        movement.Stop();

        if (targeting.HasTarget)
            movement.Face(targeting.Target);

        combat.BeginAttack();
        animationController.SetAttacking(true);
    }

    void IState.Tick()
    {
        movement.Stop();

        if (attackFinished)
            resolveAwareness();
    }

    void IState.Exit()
    {
        animationController.SetAttacking(false);
        combat.EndAttack();
    }
}
