using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerMovement : MonoBehaviour, IDamageable
{
    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float groundAcceleration = 45f;
    [SerializeField] private float groundDeceleration = 55f;
    [SerializeField] private float airAcceleration = 24f;
    [SerializeField] private float airDeceleration = 12f;
    [Range(0f, 1f)]
    [SerializeField] private float airControlMultiplier = 0.7f;
    [SerializeField] private float longitudRaycast = 0.1f;
    [SerializeField] private LayerMask floorLayer;

    [Header("Salto")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 2.2f;
    [SerializeField] private float lowJumpGravityMultiplier = 1.6f;
    [SerializeField] private float maxFallSpeed = 14f;

    [Header("Plataformas atravesables")]
    [Min(0.01f)]
    [SerializeField] private float dropThroughSpeed = 3f;
    [Min(0.01f)]
    [SerializeField] private float dropThroughMaxDuration = 1f;

    [Header("Combate")]
    [SerializeField] private float collitionForce = 6f;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudio playerAudio;

    private Health health;
    private StateMachine locomotionStateMachine;
    private StateMachine actionStateMachine;
    private PlayerLocomotion locomotion;
    private PlayerAnimationController animationController;
    private PlayerDamageReaction damageReaction;
    private PlayerSpeedModifier speedModifier;
    private PlayerGroundedState groundedState;
    private PlayerJumpState jumpState;
    private PlayerFallState fallState;
    private PlayerReadyActionState readyActionState;
    private PlayerAttackState attackState;
    private PlayerKnockbackState knockbackState;
    private PlayerDeadState deadState;

    private void Awake()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        Collider2D playerCollider = GetComponent<Collider2D>();
        PlayerInputReader inputReader = GetComponent<PlayerInputReader>();
        health = GetComponent<Health>();

        PlayerMovementPhysics movementPhysics = new PlayerMovementPhysics(
            rigidBody,
            transform,
            playerCollider
        );
        animationController = new PlayerAnimationController(animator, transform);
        speedModifier = new PlayerSpeedModifier(playerSpeed);

        PlayerLocomotion.Settings locomotionSettings = new PlayerLocomotion.Settings(
            jumpForce: jumpForce,
            groundAcceleration: groundAcceleration,
            groundDeceleration: groundDeceleration,
            airAcceleration: airAcceleration,
            airDeceleration: airDeceleration,
            airControlMultiplier: airControlMultiplier,
            raycastLength: longitudRaycast,
            floorLayer: floorLayer,
            coyoteTime: coyoteTime,
            jumpBufferTime: jumpBufferTime,
            jumpCutMultiplier: jumpCutMultiplier,
            fallGravityMultiplier: fallGravityMultiplier,
            lowJumpGravityMultiplier: lowJumpGravityMultiplier,
            maxFallSpeed: maxFallSpeed,
            dropThroughSpeed: dropThroughSpeed,
            dropThroughMaxDuration: dropThroughMaxDuration
        );

        locomotion = new PlayerLocomotion(
            inputReader,
            movementPhysics,
            animationController,
            speedModifier,
            locomotionSettings
        );
        damageReaction = new PlayerDamageReaction(
            rigidBody,
            movementPhysics,
            animationController,
            playerAudio,
            collitionForce
        );

        readyActionState = new PlayerReadyActionState(inputReader, ChangeToAttackState);
        attackState = new PlayerAttackState(animationController, playerAudio);
        deadState = new PlayerDeadState(damageReaction);
        groundedState = new PlayerGroundedState(
            locomotion,
            ChangeToJumpState,
            ChangeToFallState
        );
        jumpState = new PlayerJumpState(locomotion, animationController, ChangeToFallState);
        fallState = new PlayerFallState(
            locomotion,
            animationController,
            ChangeToGroundedState,
            ChangeToJumpState
        );
        knockbackState = new PlayerKnockbackState(
            damageReaction,
            knockbackDuration,
            ResolveLocomotionState
        );
        locomotionStateMachine = new StateMachine(groundedState);
        actionStateMachine = new StateMachine(readyActionState);

        health.OnLifeChanged += OnLifeChanged;
        health.OnDamaged += OnDamaged;
        health.OnDeath += OnDeath;
    }

    private void Start()
    {
        GameManager.Instance?.RegisterPlayer(health);
        EventManager.TriggerPlayerLifeChanged(health.Life, health.Life);
    }

    private void Update()
    {
        speedModifier.Tick(Time.deltaTime);

        if (health.IsAlive)
        {
            locomotion.UpdateGroundState();
            locomotionStateMachine.Tick();

            if (!locomotionStateMachine.IsInState(knockbackState))
                actionStateMachine.Tick();
        }

        animationController.SetGrounded(locomotion.IsGrounded);
    }

    private void OnDestroy()
    {
        locomotion?.RestorePlatformCollision();

        if (health == null) return;

        health.OnLifeChanged -= OnLifeChanged;
        health.OnDamaged -= OnDamaged;
        health.OnDeath -= OnDeath;
    }

    private void OnDisable()
    {
        locomotion?.RestorePlatformCollision();
    }

    private void ChangeToGroundedState()
    {
        locomotionStateMachine.ChangeState(groundedState);
    }

    private void ChangeToJumpState()
    {
        locomotionStateMachine.ChangeState(jumpState);
    }

    private void ChangeToFallState()
    {
        locomotionStateMachine.ChangeState(fallState);
    }

    private void ResolveLocomotionState()
    {
        if (locomotion.IsGrounded)
            ChangeToGroundedState();
        else
            ChangeToFallState();
    }

    private void ChangeToAttackState()
    {
        actionStateMachine.ChangeState(attackState);
    }

    private void ChangeToKnockbackState(Vector2 direction)
    {
        knockbackState.Configure(direction);
        locomotionStateMachine.ChangeState(knockbackState);
    }

    private void OnLifeChanged(int currentLife, int maxLife)
    {
        EventManager.TriggerPlayerLifeChanged(currentLife, maxLife);
    }

    private void OnDamaged(Vector2 attackDirection)
    {
        if (!health.IsAlive) return;

        damageReaction.PlayDamageFeedback();
        damageReaction.ShowDamageAnimation();
        StartCoroutine(ResetDamageFlag());
    }

    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(0.2f);
        OnDamageAnimationFinished();
    }

    private void OnDeath()
    {
        locomotion.RestorePlatformCollision();
        CancelCurrentAction();
        locomotionStateMachine.ChangeState(deadState);
    }

    public void OnAttackAnimationFinished()
    {
        if (!actionStateMachine.IsInState(attackState)) return;

        actionStateMachine.ChangeState(readyActionState);
        RestoreAirborneAnimation();
    }

    public void OnDamageAnimationFinished()
    {
        damageReaction?.HideDamageAnimation();
    }

    public void ApplySpeedModifier(float multiplier, float duration)
    {
        speedModifier.Apply(multiplier, duration);
    }

    public int Life => health.Life;
    public bool IsAlive => health.IsAlive;

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        if (!health.IsAlive ||
            locomotionStateMachine.IsInState(knockbackState) ||
            locomotionStateMachine.IsInState(deadState))
        {
            return;
        }

        CancelCurrentAction();
        health.TakeDamage(damage, attackDirection);

        if (health.IsAlive)
            ChangeToKnockbackState(attackDirection);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    private void CancelCurrentAction()
    {
        if (!actionStateMachine.IsInState(readyActionState))
            actionStateMachine.ChangeState(readyActionState);
    }

    private void RestoreAirborneAnimation()
    {
        if (locomotionStateMachine.IsInState(jumpState))
            animationController.PlayJump();
        else if (locomotionStateMachine.IsInState(fallState))
            animationController.PlayFall();
    }
}
