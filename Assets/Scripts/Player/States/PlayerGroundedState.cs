using System;

internal sealed class PlayerGroundedState : IState
{
    private readonly PlayerLocomotion locomotion;
    private readonly Action requestJump;
    private readonly Action requestFall;

    internal PlayerGroundedState(
        PlayerLocomotion locomotion,
        Action requestJump,
        Action requestFall)
    {
        this.locomotion = locomotion;
        this.requestJump = requestJump;
        this.requestFall = requestFall;
    }

    void IState.Enter()
    {
        locomotion.EnterGrounded();
    }

    void IState.Tick()
    {
        if (locomotion.TickGrounded())
        {
            requestJump();
            return;
        }

        if (!locomotion.IsGrounded)
        {
            requestFall();
            return;
        }

    }

    void IState.Exit()
    {
    }
}
