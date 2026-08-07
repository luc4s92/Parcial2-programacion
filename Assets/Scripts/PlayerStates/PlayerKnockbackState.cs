using UnityEngine;

public sealed class PlayerKnockbackState : IPlayerState
{
    private readonly PlayerMovement player;
    private Vector2 direction;
    private Collider2D enemyCollider;
    private float elapsedTime;

    public PlayerKnockbackState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Configure(Vector2 direction, Collider2D enemyCollider)
    {
        this.direction = direction;
        this.enemyCollider = enemyCollider;
    }

    public void Enter()
    {
        elapsedTime = 0f;
        player.BeginKnockback(direction, enemyCollider);
    }

    public void Tick()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= player.KnockbackDuration)
            player.ChangeToLocomotionState();
    }

    public void Exit()
    {
        player.EndKnockback(enemyCollider);
        enemyCollider = null;
    }
}
