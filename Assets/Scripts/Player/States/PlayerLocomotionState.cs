using System;

internal sealed class PlayerLocomotionState : IPlayerState
{
    private readonly PlayerInputReader inputReader;
    private readonly PlayerLocomotion locomotion;
    private readonly Action requestAttack;

    internal PlayerLocomotionState(
        PlayerInputReader inputReader,
        PlayerLocomotion locomotion,
        Action requestAttack)
    {
        this.inputReader = inputReader;
        this.locomotion = locomotion;
        this.requestAttack = requestAttack;
    }

    void IPlayerState.Enter()
    {
    }

    void IPlayerState.Tick()
    {
        locomotion.Tick();

        if (inputReader.AttackReleased && locomotion.IsGrounded)
            requestAttack();
    }

    void IPlayerState.Exit()
    {
    }
}
