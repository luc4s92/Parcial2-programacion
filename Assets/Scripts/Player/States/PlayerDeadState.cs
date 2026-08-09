internal sealed class PlayerDeadState : IState
{
    private readonly PlayerDamageReaction damageReaction;

    internal PlayerDeadState(PlayerDamageReaction damageReaction)
    {
        this.damageReaction = damageReaction;
    }

    void IState.Enter()
    {
        damageReaction.BeginDeath();
    }

    void IState.Tick()
    {
    }

    void IState.Exit()
    {
    }
}
