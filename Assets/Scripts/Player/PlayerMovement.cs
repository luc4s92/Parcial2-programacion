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

    [Header("Combate")]
    [SerializeField] private float collitionForce = 6f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float attackBrakeDeceleration = 90f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudio playerAudio;

    private Health health;
    private PlayerStateMachine stateMachine;
    private PlayerLocomotion locomotion;
    private PlayerAnimationController animationController;
    private PlayerDamageReaction damageReaction;
    private PlayerSpeedModifier speedModifier;
    private PlayerLocomotionState locomotionState;
    private PlayerAttackState attackState;
    private PlayerKnockbackState knockbackState;
    private PlayerDeadState deadState;

    private void Awake()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        Collider2D playerCollider = GetComponent<Collider2D>();
        PlayerInputReader inputReader = GetComponent<PlayerInputReader>();
        health = GetComponent<Health>();

        PlayerMovementPhysics movementPhysics = new PlayerMovementPhysics(rigidBody, transform);
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
            attackBrakeDeceleration: attackBrakeDeceleration
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
            playerCollider,
            movementPhysics,
            animationController,
            playerAudio,
            collitionForce
        );

        attackState = new PlayerAttackState(locomotion, animationController, playerAudio);
        deadState = new PlayerDeadState(damageReaction);
        locomotionState = new PlayerLocomotionState(inputReader, locomotion, ChangeToAttackState);
        knockbackState = new PlayerKnockbackState(
            damageReaction,
            knockbackDuration,
            ChangeToLocomotionState
        );
        stateMachine = new PlayerStateMachine(locomotionState);

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
            stateMachine.Tick();
        }

        animationController.SetGrounded(locomotion.IsGrounded);
    }

    private void OnDestroy()
    {
        if (health == null) return;

        health.OnLifeChanged -= OnLifeChanged;
        health.OnDamaged -= OnDamaged;
        health.OnDeath -= OnDeath;
    }

    private void ChangeToLocomotionState()
    {
        stateMachine.ChangeState(locomotionState);
    }

    private void ChangeToAttackState()
    {
        stateMachine.ChangeState(attackState);
    }

    private void ChangeToKnockbackState(Vector2 direction, Collider2D enemyCollider)
    {
        knockbackState.Configure(direction, enemyCollider);
        stateMachine.ChangeState(knockbackState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!health.IsAlive ||
            stateMachine.IsInState(knockbackState) ||
            stateMachine.IsInState(deadState) ||
            !collision.collider.CompareTag("Enemy"))
        {
            return;
        }

        Vector2 attackDirection = new Vector2(
            transform.position.x - collision.transform.position.x,
            0.5f
        ).normalized;

        TakeDamage(1, attackDirection);
        if (!health.IsAlive) return;

        ChangeToKnockbackState(attackDirection, collision.collider);
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
        stateMachine.ChangeState(deadState);
    }

    public void OnAttackAnimationFinished()
    {
        if (stateMachine.IsInState(attackState))
            ChangeToLocomotionState();
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
        health.TakeDamage(damage, attackDirection);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
