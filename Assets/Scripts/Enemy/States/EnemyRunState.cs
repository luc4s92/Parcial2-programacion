internal sealed class EnemyRunState : IState
{
    private readonly EnemyRun run;
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;

    internal EnemyRunState(
        EnemyRun run,
        EnemyMovement movement,
        EnemyAnimationController animationController)
    {
        this.run = run;
        this.movement = movement;
        this.animationController = animationController;
    }

    void IState.Enter()
    {
        animationController.SetMoving(true);
    }

    void IState.Tick()
    {
        run.Tick();
    }

    void IState.Exit()
    {
        movement.Stop();
        animationController.SetMoving(false);
    }
}
