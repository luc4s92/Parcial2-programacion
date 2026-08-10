using UnityEngine;

internal sealed class PlayerMovementPhysics
{
    private readonly Rigidbody2D rigidBody;
    private readonly Collider2D playerCollider;
    private readonly PlayerGroundDetector groundDetector;
    private readonly float defaultGravityScale;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private float variableJumpTimeRemaining;
    private float dropThroughTimeRemaining;
    private bool jumpWasCut;
    private Collider2D ignoredPlatformCollider;

    internal bool IsGrounded { get; private set; }
    internal Collider2D GroundCollider { get; private set; }
    internal float HorizontalSpeed => Mathf.Abs(rigidBody.linearVelocity.x);
    internal float VerticalSpeed => rigidBody.linearVelocity.y;

    internal PlayerMovementPhysics(
        Rigidbody2D rigidBody,
        Collider2D playerCollider,
        PlayerGroundDetector groundDetector)
    {
        this.rigidBody = rigidBody;
        this.playerCollider = playerCollider;
        this.groundDetector = groundDetector;
        defaultGravityScale = rigidBody.gravityScale;

        PhysicsMaterial2D movementMaterial = playerCollider.sharedMaterial;
        if (movementMaterial != null)
        {
            movementMaterial.frictionCombine = PhysicsMaterialCombine2D.Minimum;
            movementMaterial.bounceCombine = PhysicsMaterialCombine2D.Minimum;
        }
    }

    internal void UpdateGroundState(
        float groundCheckDistance,
        LayerMask floorLayer,
        float coyoteTime,
        float maxGroundAngle)
    {
        if (UpdateDropThroughPlatform())
        {
            IsGrounded = false;
            GroundCollider = null;
            coyoteCounter = 0f;
            return;
        }

        bool foundGround = groundDetector.TryGetGround(
            groundCheckDistance,
            floorLayer,
            maxGroundAngle,
            VerticalSpeed,
            out RaycastHit2D groundHit
        );
        GroundCollider = foundGround ? groundHit.collider : null;
        IsGrounded = foundGround;

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

    internal bool TryJump(float jumpForce, float variableJumpDuration)
    {
        if (jumpBufferCounter <= 0f || coyoteCounter <= 0f) return false;

        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        variableJumpTimeRemaining = variableJumpDuration;
        jumpWasCut = false;
        IsGrounded = false;
        return true;
    }

    internal void ApplyJumpRiseGravity(
        bool jumpHeld,
        float riseGravityMultiplier,
        float jumpCutMultiplier,
        float lowJumpGravityMultiplier)
    {
        if (rigidBody.linearVelocity.y <= 0f)
        {
            variableJumpTimeRemaining = 0f;
            ResetGravity();
            return;
        }

        if (!jumpHeld && variableJumpTimeRemaining > 0f && !jumpWasCut)
        {
            rigidBody.linearVelocity = new Vector2(
                rigidBody.linearVelocity.x,
                rigidBody.linearVelocity.y * jumpCutMultiplier
            );
            jumpWasCut = true;
        }

        variableJumpTimeRemaining = Mathf.Max(
            0f,
            variableJumpTimeRemaining - Time.deltaTime
        );
        float gravityMultiplier = jumpWasCut
            ? lowJumpGravityMultiplier
            : riseGravityMultiplier;
        rigidBody.gravityScale = defaultGravityScale * gravityMultiplier;
    }

    internal void ApplyFallGravity(float fallGravityMultiplier, float maxFallSpeed)
    {
        variableJumpTimeRemaining = 0f;

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

}
