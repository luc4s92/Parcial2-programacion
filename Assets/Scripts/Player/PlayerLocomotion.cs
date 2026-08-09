using UnityEngine;

internal sealed class PlayerLocomotion
{
    internal readonly struct Settings
    {
        internal readonly float JumpForce;
        internal readonly float GroundAcceleration;
        internal readonly float GroundDeceleration;
        internal readonly float AirAcceleration;
        internal readonly float AirDeceleration;
        internal readonly float AirControlMultiplier;
        internal readonly float RaycastLength;
        internal readonly LayerMask FloorLayer;
        internal readonly float CoyoteTime;
        internal readonly float JumpBufferTime;
        internal readonly float JumpCutMultiplier;
        internal readonly float FallGravityMultiplier;
        internal readonly float LowJumpGravityMultiplier;
        internal readonly float MaxFallSpeed;
        internal readonly float AttackBrakeDeceleration;

        internal Settings(
            float jumpForce,
            float groundAcceleration,
            float groundDeceleration,
            float airAcceleration,
            float airDeceleration,
            float airControlMultiplier,
            float raycastLength,
            LayerMask floorLayer,
            float coyoteTime,
            float jumpBufferTime,
            float jumpCutMultiplier,
            float fallGravityMultiplier,
            float lowJumpGravityMultiplier,
            float maxFallSpeed,
            float attackBrakeDeceleration)
        {
            JumpForce = jumpForce;
            GroundAcceleration = groundAcceleration;
            GroundDeceleration = groundDeceleration;
            AirAcceleration = airAcceleration;
            AirDeceleration = airDeceleration;
            AirControlMultiplier = airControlMultiplier;
            RaycastLength = raycastLength;
            FloorLayer = floorLayer;
            CoyoteTime = coyoteTime;
            JumpBufferTime = jumpBufferTime;
            JumpCutMultiplier = jumpCutMultiplier;
            FallGravityMultiplier = fallGravityMultiplier;
            LowJumpGravityMultiplier = lowJumpGravityMultiplier;
            MaxFallSpeed = maxFallSpeed;
            AttackBrakeDeceleration = attackBrakeDeceleration;
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
            settings.RaycastLength,
            settings.FloorLayer,
            settings.CoyoteTime
        );
    }

    internal void EnterGrounded()
    {
        movementPhysics.ResetGravity();
    }

    internal bool TickGrounded()
    {
        TickMovementAndJumpBuffer();

        bool jumped = TryJump();
        if (!jumped)
            movementPhysics.ResetGravity();

        return jumped;
    }

    internal void TickJump()
    {
        TickMovementAndJumpBuffer();
        movementPhysics.ApplyJumpRiseGravity(
            inputReader.JumpHeld,
            inputReader.JumpReleased,
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

    internal void TickDuringAttack()
    {
        if (IsGrounded)
        {
            movementPhysics.Brake(settings.AttackBrakeDeceleration);
            animationController.SetMovement(movementPhysics.HorizontalSpeed);
        }

        movementPhysics.ApplyFallGravity(
            settings.FallGravityMultiplier,
            settings.MaxFallSpeed
        );
    }

    internal void ClearJumpBuffer()
    {
        movementPhysics.ClearJumpBuffer();
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
        return movementPhysics.TryJump(settings.JumpForce);
    }
}
