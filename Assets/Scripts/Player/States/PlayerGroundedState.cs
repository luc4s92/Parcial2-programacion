using System;

internal sealed class PlayerGroundedState : IPlayerState
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

    void IPlayerState.Enter()
    {
        locomotion.EnterGrounded();
    }

    void IPlayerState.Tick()
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

    void IPlayerState.Exit()
    {
    }
}
