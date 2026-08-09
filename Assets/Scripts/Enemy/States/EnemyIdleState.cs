using System;

internal sealed class EnemyIdleState : IState
{
    private readonly EnemyTargeting targeting;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly Action requestChase;

    internal EnemyIdleState(
        EnemyTargeting targeting,
        EnemyMovement movement,
        EnemyAnimationController animationController,
        Action requestChase)
    {
        this.targeting = targeting;
        this.movement = movement;
        this.animationController = animationController;
        this.requestChase = requestChase;
    }

    void IState.Enter()
    {
        movement.Stop();
        animationController.SetMoving(false);
    }

    void IState.Tick()
    {
        if (targeting.IsTargetDetected)
            requestChase();
    }

    void IState.Exit()
    {
    }
}
