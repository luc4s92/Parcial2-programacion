using System;

internal sealed class EnemyRangedAttackState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly EnemyRangedCombat combat;
    private readonly Action requestIdle;

    internal EnemyRangedAttackState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        EnemyRangedCombat combat,
        Action requestIdle)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.combat = combat;
        this.requestIdle = requestIdle;
    }

    void IState.Enter()
    {
        movement.StopCompletely();
        animationController.SetMoving(false);
    }

    void IState.Tick()
    {
        movement.StopCompletely();

        if (!targeting.IsTargetDetected)
        {
            requestIdle();
            return;
        }

        movement.Face(targeting.Target);
        combat.TryFire(targeting.Target);
    }

    void IState.Exit()
    {
    }
}
