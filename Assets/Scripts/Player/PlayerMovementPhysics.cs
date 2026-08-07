using UnityEngine;

public sealed class PlayerMovementPhysics
{
    private readonly Rigidbody2D rigidBody;
    private readonly Transform transform;
    private readonly float defaultGravityScale;

    private float coyoteCounter;
    private float jumpBufferCounter;

    public bool IsGrounded { get; private set; }
    public float HorizontalSpeed => Mathf.Abs(rigidBody.linearVelocity.x);

    public PlayerMovementPhysics(Rigidbody2D rigidBody, Transform transform)
    {
        this.rigidBody = rigidBody;
        this.transform = transform;
        defaultGravityScale = rigidBody.gravityScale;
    }

    public void UpdateGroundState(float raycastLength, LayerMask floorLayer, float coyoteTime)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, floorLayer);
        IsGrounded = hit.collider != null;

        if (IsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }

    public void UpdateJumpBuffer(bool jumpPressed, float jumpBufferTime)
    {
        if (jumpPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    public void ClearJumpBuffer()
    {
        jumpBufferCounter = 0f;
    }

    public void MoveHorizontally(
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

    public void TryJump(float jumpForce)
    {
        if (jumpBufferCounter <= 0f || coyoteCounter <= 0f) return;

        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    public void ApplyJumpGravity(
        bool jumpHeld,
        bool jumpReleased,
        float jumpCutMultiplier,
        float fallGravityMultiplier,
        float lowJumpGravityMultiplier,
        float maxFallSpeed)
    {
        if (IsGrounded)
        {
            ResetGravity();
            return;
        }

        if (rigidBody.linearVelocity.y < 0f)
        {
            rigidBody.gravityScale = defaultGravityScale * fallGravityMultiplier;
            if (rigidBody.linearVelocity.y < -maxFallSpeed)
                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, -maxFallSpeed);
        }
        else if (rigidBody.linearVelocity.y > 0f && !jumpHeld)
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

    public void Brake(float deceleration)
    {
        float horizontalVelocity = Mathf.MoveTowards(
            rigidBody.linearVelocity.x,
            0f,
            deceleration * Time.deltaTime
        );

        rigidBody.linearVelocity = new Vector2(horizontalVelocity, rigidBody.linearVelocity.y);
    }

    public void ResetGravity()
    {
        rigidBody.gravityScale = defaultGravityScale;
    }

    public void Stop()
    {
        rigidBody.linearVelocity = Vector2.zero;
    }
}
