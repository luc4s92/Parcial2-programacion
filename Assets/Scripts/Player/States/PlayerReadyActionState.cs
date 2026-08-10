using System;

internal sealed class PlayerReadyActionState : IState
{
    private readonly PlayerInputReader inputReader;
    private readonly Action requestAttack;
    private readonly Func<bool> canRequestRangedAttack;
    private readonly Action requestRangedAttack;

    internal PlayerReadyActionState(
        PlayerInputReader inputReader,
        Action requestAttack,
        Func<bool> canRequestRangedAttack,
        Action requestRangedAttack)
    {
        this.inputReader = inputReader;
        this.requestAttack = requestAttack;
        this.canRequestRangedAttack = canRequestRangedAttack;
        this.requestRangedAttack = requestRangedAttack;
    }

    void IState.Enter()
    {
    }

    void IState.Tick()
    {
        if (inputReader.AttackPressed)
        {
            requestAttack();
            return;
        }

        if (inputReader.RangedAttackPressed && canRequestRangedAttack())
            requestRangedAttack();
    }

    void IState.Exit()
    {
    }
}
