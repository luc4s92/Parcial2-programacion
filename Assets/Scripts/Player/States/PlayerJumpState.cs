using System;

internal sealed class PlayerJumpState : IState
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

    void IState.Enter()
    {
        animationController.PlayJump();
    }

    void IState.Tick()
    {
        locomotion.TickJump();

        if (locomotion.VerticalSpeed <= 0f)
            requestFall();
    }

    void IState.Exit()
    {
    }
}
