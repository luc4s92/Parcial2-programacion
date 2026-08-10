using UnityEngine;

internal sealed class PlayerLocomotion
{
    internal enum GroundedTickResult
    {
        None,
        Jumped,
        DroppedThroughPlatform
    }

    internal readonly struct Settings
    {
        internal readonly float JumpForce;
        internal readonly float GroundAcceleration;
        internal readonly float GroundDeceleration;
        internal readonly float AirAcceleration;
        internal readonly float AirDeceleration;
        internal readonly float AirControlMultiplier;
        internal readonly float GroundCheckDistance;
        internal readonly float MaxGroundAngle;
        internal readonly LayerMask FloorLayer;
        internal readonly float CoyoteTime;
        internal readonly float JumpBufferTime;
        internal readonly float VariableJumpDuration;
        internal readonly float RiseGravityMultiplier;
        internal readonly float JumpCutMultiplier;
        internal readonly float FallGravityMultiplier;
        internal readonly float LowJumpGravityMultiplier;
        internal readonly float MaxFallSpeed;
        internal readonly float DropThroughSpeed;
        internal readonly float DropThroughMaxDuration;

        internal Settings(
            float jumpForce,
            float groundAcceleration,
            float groundDeceleration,
            float airAcceleration,
            float airDeceleration,
            float airControlMultiplier,
            float groundCheckDistance,
            float maxGroundAngle,
            LayerMask floorLayer,
            float coyoteTime,
            float jumpBufferTime,
            float variableJumpDuration,
            float riseGravityMultiplier,
            float jumpCutMultiplier,
            float fallGravityMultiplier,
            float lowJumpGravityMultiplier,
            float maxFallSpeed,
            float dropThroughSpeed,
            float dropThroughMaxDuration)
        {
            JumpForce = jumpForce;
            GroundAcceleration = groundAcceleration;
            GroundDeceleration = groundDeceleration;
            AirAcceleration = airAcceleration;
            AirDeceleration = airDeceleration;
            AirControlMultiplier = airControlMultiplier;
            GroundCheckDistance = groundCheckDistance;
            MaxGroundAngle = maxGroundAngle;
            FloorLayer = floorLayer;
            CoyoteTime = coyoteTime;
            JumpBufferTime = jumpBufferTime;
            VariableJumpDuration = variableJumpDuration;
            RiseGravityMultiplier = riseGravityMultiplier;
            JumpCutMultiplier = jumpCutMultiplier;
            FallGravityMultiplier = fallGravityMultiplier;
            LowJumpGravityMultiplier = lowJumpGravityMultiplier;
            MaxFallSpeed = maxFallSpeed;
            DropThroughSpeed = dropThroughSpeed;
            DropThroughMaxDuration = dropThroughMaxDuration;
        }
    }

    private readonly PlayerInputReader inputReader;
    private readonly PlayerMovementPhysics movementPhysics;
    private readonly PlayerAnimationController animationController;
    private readonly PlayerSpeedModifier speedModifier;
    private readonly Settings settings;

    internal bool IsGrounded => movementPhysics.IsGrounded;
    internal float VerticalSpeed => movementPhysics.VerticalSpeed;

    internal PlayerLocomotion(
        PlayerInputReader inputReader,
        PlayerMovementPhysics movementPhysics,
        PlayerAnimationController animationController,
        PlayerSpeedModifier speedModifier,
        Settings settings)
    {
        this.inputReader = inputReader;
        this.movementPhysics = movementPhysics;
        this.animationController = animationController;
        this.speedModifier = speedModifier;
        this.settings = settings;
    }

    internal void UpdateGroundState()
    {
        movementPhysics.UpdateGroundState(
            settings.GroundCheckDistance,
            settings.FloorLayer,
            settings.CoyoteTime,
            settings.MaxGroundAngle
        );
    }

    internal void EnterGrounded()
    {
        movementPhysics.ResetGravity();
    }

    internal GroundedTickResult TickGrounded()
    {
        TickMovementAndJumpBuffer();

        if (TryDropThroughPlatform())
            return GroundedTickResult.DroppedThroughPlatform;

        if (TryJump())
            return GroundedTickResult.Jumped;

        movementPhysics.ResetGravity();
        return GroundedTickResult.None;
    }

    internal void TickJump()
    {
        TickMovementAndJumpBuffer();
        movementPhysics.ApplyJumpRiseGravity(
            inputReader.JumpHeld,
            settings.RiseGravityMultiplier,
            settings.JumpCutMultiplier,
            settings.LowJumpGravityMultiplier
        );
    }

    internal bool TickFall()
    {
        TickMovementAndJumpBuffer();

        if (TryJump())
            return true;

        movementPhysics.ApplyFallGravity(
            settings.FallGravityMultiplier,
            settings.MaxFallSpeed
        );
        return false;
    }

    private void MoveHorizontally()
    {
        float horizontalInput = inputReader.MoveX;
        movementPhysics.MoveHorizontally(
            horizontalInput,
            speedModifier.CurrentSpeed,
            settings.GroundAcceleration,
            settings.GroundDeceleration,
            settings.AirAcceleration,
            settings.AirDeceleration,
            settings.AirControlMultiplier
        );

        animationController.SetMovement(movementPhysics.HorizontalSpeed);
        animationController.FaceMovement(horizontalInput);
    }

    private void TickMovementAndJumpBuffer()
    {
        MoveHorizontally();
        movementPhysics.UpdateJumpBuffer(inputReader.JumpPressed, settings.JumpBufferTime);
    }

    private bool TryJump()
    {
        return movementPhysics.TryJump(
            settings.JumpForce,
            settings.VariableJumpDuration
        );
    }

    private bool TryDropThroughPlatform()
    {
        if (!inputReader.DownHeld || !inputReader.JumpPressed)
            return false;

        Collider2D groundCollider = movementPhysics.GroundCollider;
        if (groundCollider == null || !groundCollider.TryGetComponent(out OneWayPlatform _))
            return false;

        movementPhysics.BeginDropThroughPlatform(
            groundCollider,
            settings.DropThroughSpeed,
            settings.DropThroughMaxDuration
        );
        return true;
    }

    internal void RestorePlatformCollision()
    {
        movementPhysics.RestorePlatformCollision();
    }
}
