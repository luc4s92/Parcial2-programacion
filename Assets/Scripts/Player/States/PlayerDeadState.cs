internal sealed class PlayerDeadState : IPlayerState
{
    private readonly PlayerDamageReaction damageReaction;

    internal PlayerDeadState(PlayerDamageReaction damageReaction)
    {
        this.damageReaction = damageReaction;
    }

    void IPlayerState.Enter()
    {
        damageReaction.BeginDeath();
    }

    void IPlayerState.Tick()
    {
    }

    void IPlayerState.Exit()
    {
    }
}
