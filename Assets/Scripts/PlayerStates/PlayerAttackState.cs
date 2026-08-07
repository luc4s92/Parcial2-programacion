public sealed class PlayerAttackState : IPlayerState
{
    private readonly PlayerMovement player;

    public PlayerAttackState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.BeginAttack();
    }

    public void Tick()
    {
        player.TickAttack();
    }

    public void Exit()
    {
        player.EndAttack();
    }
}
