internal sealed class PlayerAttackState : IState
{
    private readonly PlayerLocomotion locomotion;
    private readonly PlayerAnimationController animationController;
    private readonly PlayerAudio playerAudio;

    internal PlayerAttackState(
        PlayerLocomotion locomotion,
        PlayerAnimationController animationController,
        PlayerAudio playerAudio)
    {
        this.locomotion = locomotion;
        this.animationController = animationController;
        this.playerAudio = playerAudio;
    }

    void IState.Enter()
    {
        locomotion.ClearJumpBuffer();
        animationController.SetAttacking(true);
        playerAudio?.PlaySwing();
    }

    void IState.Tick()
    {
        locomotion.TickDuringAttack();
    }

    void IState.Exit()
    {
        animationController.SetAttacking(false);
    }
}
