internal sealed class PlayerAttackState : IState
{
    private readonly PlayerAnimationController animationController;
    private readonly PlayerAudio playerAudio;

    internal PlayerAttackState(
        PlayerAnimationController animationController,
        PlayerAudio playerAudio)
    {
        this.animationController = animationController;
        this.playerAudio = playerAudio;
    }

    void IState.Enter()
    {
        animationController.SetAttacking(true);
        playerAudio?.PlaySwing();
    }

    void IState.Tick()
    {
    }

    void IState.Exit()
    {
        animationController.SetAttacking(false);
    }
}
