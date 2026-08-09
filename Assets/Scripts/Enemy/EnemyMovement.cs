using UnityEngine;

internal sealed class EnemyMovement
{
    private readonly Rigidbody2D rigidBody;
    private readonly Transform owner;
    private readonly float speed;
    private readonly bool facesRightByDefault;

    internal EnemyMovement(
        Rigidbody2D rigidBody,
        Transform owner,
        float speed,
        bool facesRightByDefault)
    {
        this.rigidBody = rigidBody;
        this.owner = owner;
        this.speed = speed;
        this.facesRightByDefault = facesRightByDefault;
    }

    internal void Chase(Transform target)
    {
        float directionX = Mathf.Sign(target.position.x - owner.position.x);
        Move(directionX);
    }

    internal void Face(Transform target)
    {
        float directionX = target.position.x - owner.position.x;
        FaceDirection(directionX);
    }

    internal void Move(float directionX)
    {
        rigidBody.linearVelocity = new Vector2(
            Mathf.Sign(directionX) * speed,
            rigidBody.linearVelocity.y
        );
    }

    internal void FaceDirection(float directionX)
    {
        if (Mathf.Abs(directionX) <= 0.01f) return;

        Vector3 scale = owner.localScale;
        float scaleMagnitude = Mathf.Abs(scale.x);
        bool shouldFaceRight = directionX > 0f;
        scale.x = shouldFaceRight == facesRightByDefault
            ? scaleMagnitude
            : -scaleMagnitude;
        owner.localScale = scale;
    }

    internal void ApplyKnockback(Vector2 direction, float force)
    {
        Stop();

        if (force <= 0f) return;

        rigidBody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    internal void Stop()
    {
        rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
    }

    internal void StopCompletely()
    {
        rigidBody.linearVelocity = Vector2.zero;
    }
}
