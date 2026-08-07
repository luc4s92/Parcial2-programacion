public sealed class PlayerLocomotionState : IPlayerState
{
    private readonly PlayerMovement player;

    public PlayerLocomotionState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Enter()
    {
    }

    public void Tick()
    {
        player.TickLocomotion();

        if (player.CanStartAttack())
            player.ChangeToAttackState();
    }

    public void Exit()
    {
    }
}
