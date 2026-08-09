using System;

internal sealed class EnemyDeadState : IState
{
    private readonly EnemyDamageReaction damageReaction;
    private readonly Action notifyDeath;

    internal EnemyDeadState(EnemyDamageReaction damageReaction, Action notifyDeath)
    {
        this.damageReaction = damageReaction;
        this.notifyDeath = notifyDeath;
    }

    void IState.Enter()
    {
        damageReaction.BeginDeath();
        notifyDeath();
    }

    void IState.Tick()
    {
    }

    void IState.Exit()
    {
    }
}
