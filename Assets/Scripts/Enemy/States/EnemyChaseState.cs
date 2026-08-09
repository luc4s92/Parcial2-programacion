using System;

internal sealed class EnemyChaseState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly EnemyCombat combat;
    private readonly Action requestIdle;
    private readonly Action requestAttack;

    internal EnemyChaseState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        EnemyCombat combat,
        Action requestIdle,
        Action requestAttack)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.combat = combat;
        this.requestIdle = requestIdle;
        this.requestAttack = requestAttack;
    }

    void IState.Enter()
    {
    }

    void IState.Tick()
    {
        if (!targeting.IsTargetDetected)
        {
            requestIdle();
            return;
        }

        movement.Face(targeting.Target);

        if (targeting.IsTargetInAttackRange)
        {
            movement.Stop();
            animationController.SetMoving(false);

            if (combat.CanAttack)
                requestAttack();

            return;
        }

        movement.Chase(targeting.Target);
        animationController.SetMoving(true);
    }

    void IState.Exit()
    {
        animationController.SetMoving(false);
    }
}
