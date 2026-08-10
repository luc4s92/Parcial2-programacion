using UnityEngine;

internal sealed class PlayerMovementPhysics
{
    private readonly Rigidbody2D rigidBody;
    private readonly Transform transform;
    private readonly Collider2D playerCollider;
    private readonly float defaultGravityScale;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private float dropThroughTimeRemaining;
    private Collider2D ignoredPlatformCollider;

    internal bool IsGrounded { get; private set; }
    internal Collider2D GroundCollider { get; private set; }
    internal float HorizontalSpeed => Mathf.Abs(rigidBody.linearVelocity.x);
    internal float VerticalSpeed => rigidBody.linearVelocity.y;

    internal PlayerMovementPhysics(
        Rigidbody2D rigidBody,
        Transform transform,
        Collider2D playerCollider)
    {
        this.rigidBody = rigidBody;
        this.transform = transform;
        this.playerCollider = playerCollider;
        defaultGravityScale = rigidBody.gravityScale;
    }

    internal void UpdateGroundState(float raycastLength, LayerMask floorLayer, float coyoteTime)
    {
        if (UpdateDropThroughPlatform())
        {
            IsGrounded = false;
            GroundCollider = null;
            coyoteCounter = 0f;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, floorLayer);
        GroundCollider = IsValidGroundHit(hit) ? hit.collider : null;
        IsGrounded = GroundCollider != null;

        if (IsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }

    internal void UpdateJumpBuffer(bool jumpPressed, float jumpBufferTime)
    {
        if (jumpPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    internal void ClearJumpBuffer()
    {
        jumpBufferCounter = 0f;
    }

    internal void BeginDropThroughPlatform(
        Collider2D platformCollider,
        float dropSpeed,
        float maxDuration)
    {
        RestorePlatformCollision();

        ignoredPlatformCollider = platformCollider;
        dropThroughTimeRemaining = maxDuration;
        Physics2D.IgnoreCollision(playerCollider, ignoredPlatformCollider, true);

        rigidBody.linearVelocity = new Vector2(
            rigidBody.linearVelocity.x,
            -Mathf.Abs(dropSpeed)
        );

        ClearJumpBuffer();
        coyoteCounter = 0f;
        IsGrounded = false;
        GroundCollider = null;
    }

    internal void RestorePlatformCollision()
    {
        if (ignoredPlatformCollider == null)
            return;

        if (playerCollider != null &&
            playerCollider.isActiveAndEnabled &&
            ignoredPlatformCollider.isActiveAndEnabled)
        {
            Physics2D.IgnoreCollision(playerCollider, ignoredPlatformCollider, false);
        }

        ignoredPlatformCollider = null;
        dropThroughTimeRemaining = 0f;
    }

    internal void MoveHorizontally(
        float inputX,
        float moveSpeed,
        float groundAcceleration,
        float groundDeceleration,
        float airAcceleration,
        float airDeceleration,
        float airControlMultiplier)
    {
        float targetSpeed = inputX * moveSpeed;
        float accelerationRate;

        if (Mathf.Abs(inputX) > 0.01f)
            accelerationRate = IsGrounded ? groundAcceleration : airAcceleration * airControlMultiplier;
        else
            accelerationRate = IsGrounded ? groundDeceleration : airDeceleration;

        float horizontalVelocity = Mathf.MoveTowards(
            rigidBody.linearVelocity.x,
            targetSpeed,
            accelerationRate * Time.deltaTime
        );

        rigidBody.linearVelocity = new Vector2(horizontalVelocity, rigidBody.linearVelocity.y);
    }

    internal bool TryJump(float jumpForce)
    {
        if (jumpBufferCounter <= 0f || coyoteCounter <= 0f) return false;

        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        IsGrounded = false;
        return true;
    }

    internal void ApplyJumpRiseGravity(
        bool jumpHeld,
        bool jumpReleased,
        float jumpCutMultiplier,
        float lowJumpGravityMultiplier)
    {
        if (rigidBody.linearVelocity.y > 0f && !jumpHeld)
        {
            rigidBody.gravityScale = defaultGravityScale * lowJumpGravityMultiplier;
        }
        else
        {
            ResetGravity();
        }

        if (jumpReleased && rigidBody.linearVelocity.y > 0f)
        {
            rigidBody.linearVelocity = new Vector2(
                rigidBody.linearVelocity.x,
                rigidBody.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    internal void ApplyFallGravity(float fallGravityMultiplier, float maxFallSpeed)
    {
        if (IsGrounded)
        {
            ResetGravity();
            return;
        }

        rigidBody.gravityScale = defaultGravityScale * fallGravityMultiplier;

        if (rigidBody.linearVelocity.y < -maxFallSpeed)
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, -maxFallSpeed);
    }

    internal void Brake(float deceleration)
    {
        float horizontalVelocity = Mathf.MoveTowards(
            rigidBody.linearVelocity.x,
            0f,
            deceleration * Time.deltaTime
        );

        rigidBody.linearVelocity = new Vector2(horizontalVelocity, rigidBody.linearVelocity.y);
    }

    internal void ResetGravity()
    {
        rigidBody.gravityScale = defaultGravityScale;
    }

    internal void Stop()
    {
        rigidBody.linearVelocity = Vector2.zero;
    }

    private bool UpdateDropThroughPlatform()
    {
        if (ignoredPlatformCollider == null)
            return false;

        if (!playerCollider.isActiveAndEnabled || !ignoredPlatformCollider.isActiveAndEnabled)
        {
            ignoredPlatformCollider = null;
            dropThroughTimeRemaining = 0f;
            return false;
        }

        dropThroughTimeRemaining -= Time.deltaTime;

        bool clearedPlatform = playerCollider.bounds.max.y < ignoredPlatformCollider.bounds.min.y;
        if (clearedPlatform || dropThroughTimeRemaining <= 0f)
        {
            RestorePlatformCollision();
            return false;
        }

        return true;
    }

    private bool IsValidGroundHit(RaycastHit2D hit)
    {
        if (hit.collider == null)
            return false;

        if (!hit.collider.TryGetComponent(out OneWayPlatform _))
            return true;

        const float surfaceTolerance = 0.05f;
        bool isFallingOrStill = rigidBody.linearVelocity.y <= 0f;
        bool feetAreAboveSurface =
            playerCollider.bounds.min.y >= hit.collider.bounds.max.y - surfaceTolerance;

        return isFallingOrStill && feetAreAboveSurface;
    }
}
