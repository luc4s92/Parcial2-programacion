using UnityEngine;

internal sealed class EnemyDamageReaction
{
    private readonly EnemyMovement movement;
    private readonly EnemyAnimationController animationController;
    private readonly EnemyAudio audio;
    private readonly float knockbackForce;

    internal EnemyDamageReaction(
        EnemyMovement movement,
        EnemyAnimationController animationController,
        EnemyAudio audio,
        float knockbackForce)
    {
        this.movement = movement;
        this.animationController = animationController;
        this.audio = audio;
        this.knockbackForce = knockbackForce;
    }

    internal void BeginHit(Vector2 direction)
    {
        animationController.SetDamaged(true);
        audio?.PlayHit();
        movement.ApplyKnockback(direction, knockbackForce);
    }

    internal void EndHit()
    {
        animationController.SetDamaged(false);
        movement.StopCompletely();
    }

    internal void BeginDeath()
    {
        animationController.SetMoving(false);
        animationController.SetDamaged(false);
        animationController.SetAttacking(false);
        animationController.SetDead();
        movement.StopCompletely();
        audio?.PlayDeath();
    }
}
