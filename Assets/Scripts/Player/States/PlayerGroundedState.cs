using System;

internal sealed class PlayerGroundedState : IState
{
    private readonly PlayerInputReader inputReader;
    private readonly PlayerLocomotion locomotion;
    private readonly Action requestAttack;
    private readonly Action requestJump;
    private readonly Action requestFall;

    internal PlayerGroundedState(
        PlayerInputReader inputReader,
        PlayerLocomotion locomotion,
        Action requestAttack,
        Action requestJump,
        Action requestFall)
    {
        this.inputReader = inputReader;
        this.locomotion = locomotion;
        this.requestAttack = requestAttack;
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

        if (inputReader.AttackReleased)
            requestAttack();
    }

    void IState.Exit()
    {
    }
}
