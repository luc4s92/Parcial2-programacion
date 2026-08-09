using System;

internal sealed class EnemyRangedIdleState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly Action requestAttack;

    internal EnemyRangedIdleState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        Action requestAttack)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.requestAttack = requestAttack;
    }

    void IState.Enter()
    {
        movement.StopCompletely();
        animationController.SetMoving(false);
    }

    void IState.Tick()
    {
        if (targeting.IsTargetDetected)
            requestAttack();
    }

    void IState.Exit()
    {
    }
}
