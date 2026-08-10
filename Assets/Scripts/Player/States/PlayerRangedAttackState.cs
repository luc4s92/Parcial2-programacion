internal sealed class PlayerRangedAttackState : IState
{
    private readonly PlayerAnimationController animationController;

    internal PlayerRangedAttackState(PlayerAnimationController animationController)
    {
        this.animationController = animationController;
    }

    void IState.Enter()
    {
        animationController.PlayRangedAttack();
    }

    void IState.Tick()
    {
    }

    void IState.Exit()
    {
        animationController.StopRangedAttack();
    }
}
