using System;

internal sealed class PlayerReadyActionState : IState
{
    private readonly PlayerInputReader inputReader;
    private readonly Action requestAttack;

    internal PlayerReadyActionState(
        PlayerInputReader inputReader,
        Action requestAttack)
    {
        this.inputReader = inputReader;
        this.requestAttack = requestAttack;
    }

    void IState.Enter()
    {
    }

    void IState.Tick()
    {
        if (inputReader.AttackPressed)
            requestAttack();
    }

    void IState.Exit()
    {
    }
}
