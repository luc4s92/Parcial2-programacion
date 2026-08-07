using UnityEngine;

internal sealed class PlayerDamageReaction
{
    private readonly Rigidbody2D rigidBody;
    private readonly Collider2D playerCollider;
    private readonly PlayerMovementPhysics movementPhysics;
    private readonly PlayerAnimationController animationController;
    private readonly PlayerAudio playerAudio;
    private readonly float collisionForce;

    internal PlayerDamageReaction(
        Rigidbody2D rigidBody,
        Collider2D playerCollider,
        PlayerMovementPhysics movementPhysics,
        PlayerAnimationController animationController,
        PlayerAudio playerAudio,
        float collisionForce)
    {
        this.rigidBody = rigidBody;
        this.playerCollider = playerCollider;
        this.movementPhysics = movementPhysics;
        this.animationController = animationController;
        this.playerAudio = playerAudio;
        this.collisionForce = collisionForce;
    }

    internal void PlayDamageFeedback()
    {
        playerAudio?.PlayDamage();
    }

    internal void ShowDamageAnimation()
    {
        animationController.SetDamaged(true);
    }

    internal void HideDamageAnimation()
    {
        animationController.SetDamaged(false);
    }

    internal void BeginKnockback(Vector2 direction, Collider2D enemyCollider)
    {
        SetEnemyCollisionIgnored(enemyCollider, true);
        movementPhysics.ResetGravity();
        movementPhysics.Stop();

        Vector2 force = new Vector2(
            direction.x * collisionForce,
            direction.y * (collisionForce * 0.5f)
        );
        rigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    internal void EndKnockback(Collider2D enemyCollider)
    {
        SetEnemyCollisionIgnored(enemyCollider, false);
        movementPhysics.Stop();
    }

    internal void BeginDeath()
    {
        animationController.SetDead();
        movementPhysics.ResetGravity();
        movementPhysics.Stop();
        playerAudio?.PlayDeath();
    }

    private void SetEnemyCollisionIgnored(Collider2D enemyCollider, bool ignored)
    {
        if (enemyCollider != null)
            Physics2D.IgnoreCollision(playerCollider, enemyCollider, ignored);
    }
}
