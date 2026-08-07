public sealed class PlayerDeadState : IPlayerState
{
    private readonly PlayerMovement player;

    public PlayerDeadState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.BeginDeath();
    }

    public void Tick()
    {
    }

    public void Exit()
    {
    }
}
