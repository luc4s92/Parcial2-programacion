using System;

internal sealed class PlayerFallState : IState
{
    private readonly PlayerLocomotion locomotion;
    private readonly PlayerAnimationController animationController;
    private readonly Action requestGrounded;
    private readonly Action requestJump;

    internal PlayerFallState(
        PlayerLocomotion locomotion,
        PlayerAnimationController animationController,
        Action requestGrounded,
        Action requestJump)
    {
        this.locomotion = locomotion;
        this.animationController = animationController;
        this.requestGrounded = requestGrounded;
        this.requestJump = requestJump;
    }

    void IState.Enter()
    {
        animationController.PlayFall();
    }

    void IState.Tick()
    {
        if (locomotion.TickFall())
        {
            requestJump();
            return;
        }

        if (locomotion.IsGrounded)
            requestGrounded();
    }

    void IState.Exit()
    {
    }
}
