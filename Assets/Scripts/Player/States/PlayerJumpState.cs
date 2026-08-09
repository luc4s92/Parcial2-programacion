using System;

internal sealed class PlayerJumpState : IPlayerState
{
    private readonly PlayerLocomotion locomotion;
    private readonly PlayerAnimationController animationController;
    private readonly Action requestFall;

    internal PlayerJumpState(
        PlayerLocomotion locomotion,
        PlayerAnimationController animationController,
        Action requestFall)
    {
        this.locomotion = locomotion;
        this.animationController = animationController;
        this.requestFall = requestFall;
    }

    void IPlayerState.Enter()
    {
        animationController.PlayJump();
    }

    void IPlayerState.Tick()
    {
        locomotion.TickJump();

        if (locomotion.VerticalSpeed <= 0f)
            requestFall();
    }

    void IPlayerState.Exit()
    {
    }
}
