internal sealed class PlayerAttackState : IPlayerState
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

    void IPlayerState.Enter()
    {
        locomotion.ClearJumpBuffer();
        animationController.SetAttacking(true);
        playerAudio?.PlaySwing();
    }

    void IPlayerState.Tick()
    {
        locomotion.TickDuringAttack();
    }

    void IPlayerState.Exit()
    {
        animationController.SetAttacking(false);
    }
}
