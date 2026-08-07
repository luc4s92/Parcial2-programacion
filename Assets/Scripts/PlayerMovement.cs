using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
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

    private Rigidbody2D rigidBody;
    private Collider2D myCollider;
    private Health health;
    private PlayerMovementPhysics movementPhysics;
    private PlayerStateMachine stateMachine;
    private PlayerLocomotionState locomotionState;
    private PlayerAttackState attackState;
    private PlayerKnockbackState knockbackState;
    private PlayerDeadState deadState;
    private bool onFloor;

    // ---------------- POWER UPS ----------------
    private float baseSpeed;                 // Velocidad original
    public float MoveSpeed { get; private set; } // Velocidad actual
    private Coroutine speedModifierRoutine;
    private float currentMultiplier = 1f;
    private float remainingDuration = 0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        movementPhysics = new PlayerMovementPhysics(rigidBody, transform);
        locomotionState = new PlayerLocomotionState(this);
        attackState = new PlayerAttackState(this);
        knockbackState = new PlayerKnockbackState(this);
        deadState = new PlayerDeadState(this);
        stateMachine = new PlayerStateMachine(locomotionState);

        health.OnLifeChanged += OnLifeChanged;
        health.OnDeath += OnDeath;

        baseSpeed = playerSpeed; // Guardamos velocidad original
        MoveSpeed = baseSpeed;
    }

    private void Start()
    {
        GameManager.Instance?.RegisterPlayer(health);
        EventManager.TriggerPlayerLifeChanged(health.Life, health.Life);
    }

    private void Update()
    {
        if (health.IsAlive)
        {
            movementPhysics.UpdateGroundState(longitudRaycast, floorLayer, coyoteTime);
            onFloor = movementPhysics.IsGrounded;

            stateMachine.Tick();
        }

        animator.SetBool("onfloor", onFloor);
    }

    public float KnockbackDuration => knockbackDuration;

    public void ChangeToLocomotionState()
    {
        stateMachine.ChangeState(locomotionState);
    }

    public void ChangeToAttackState()
    {
        stateMachine.ChangeState(attackState);
    }

    private void ChangeToKnockbackState(Vector2 direction, Collider2D enemyCollider)
    {
        knockbackState.Configure(direction, enemyCollider);
        stateMachine.ChangeState(knockbackState);
    }

    private void ChangeToDeadState()
    {
        stateMachine.ChangeState(deadState);
    }

    public bool CanStartAttack()
    {
        return Input.GetKeyUp(KeyCode.Z) && onFloor;
    }

    public void TickLocomotion()
    {
        HandleMovement();
        HandleJump();
    }

    public void TickAttack()
    {
        ApplyAttackBrake();
        ApplyJumpGravity();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        movementPhysics.MoveHorizontally(
            inputX,
            MoveSpeed,
            groundAcceleration,
            groundDeceleration,
            airAcceleration,
            airDeceleration,
            airControlMultiplier
        );

        animator.SetFloat("movement", movementPhysics.HorizontalSpeed);

        if (inputX < 0) transform.localScale = new Vector3(-1, 1, 1);
        if (inputX > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    private void UpdateJumpTimers()
    {
        movementPhysics.UpdateJumpBuffer(Input.GetKeyDown(KeyCode.Space), jumpBufferTime);
    }

    private void TryJump()
    {
        movementPhysics.TryJump(jumpForce);
    }

    private void HandleJump()
    {
        UpdateJumpTimers();
        TryJump();
        ApplyJumpGravity();
    }

    private void ApplyJumpGravity()
    {
        movementPhysics.ApplyJumpGravity(
            Input.GetKey(KeyCode.Space),
            Input.GetKeyUp(KeyCode.Space),
            jumpCutMultiplier,
            fallGravityMultiplier,
            lowJumpGravityMultiplier,
            maxFallSpeed
        );
    }

    private void ApplyAttackBrake()
    {
        if (!onFloor) return;

        movementPhysics.Brake(attackBrakeDeceleration);
        animator.SetFloat("movement", movementPhysics.HorizontalSpeed);
    }

    public void Atacking()
    {
        ChangeToAttackState();
    }

    public void BeginAttack()
    {
        movementPhysics.ClearJumpBuffer();
        animator.SetBool("atack", true);
        playerAudio?.PlaySwing();
    }

    public void EndAttack()
    {
        animator.SetBool("atack", false);
    }

    public void DeactivateAtacking()
    {
        if (stateMachine.IsInState(attackState))
            ChangeToLocomotionState();
    }

    // ----------------- Danio y rebote -----------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!health.IsAlive || stateMachine.IsInState(knockbackState) || stateMachine.IsInState(deadState)) return;

        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("[PlayerMovement] Colision con Enemy");

            Vector2 attackDir = new Vector2(
                transform.position.x - collision.transform.position.x,
                0.5f
            ).normalized;

            TakeDamage(1, attackDir);

            if (health.IsAlive)
            {
                playerAudio?.PlayDamage();
                ChangeToKnockbackState(attackDir, collision.collider);
            }
        }
    }

    public void BeginKnockback(Vector2 direction, Collider2D enemyCollider)
    {
        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, true);

        movementPhysics.ResetGravity();
        movementPhysics.Stop();

        Vector2 knockbackForce = new Vector2(direction.x * collitionForce, direction.y * (collitionForce * 0.5f));
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    public void EndKnockback(Collider2D enemyCollider)
    {
        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, false);

        movementPhysics.Stop();
    }

    private void OnLifeChanged(int currentLife, int maxLife, Vector2 attackDirection)
    {
        EventManager.TriggerPlayerLifeChanged(currentLife, maxLife);

        if (!health.IsAlive) return;

        animator.SetBool("damage", true);
        StartCoroutine(ResetDamageFlag());
    }

    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("damage", false);
    }

    private void OnDeath()
    {
        ChangeToDeadState();
    }

    public void BeginDeath()
    {
        animator.SetBool("death", true);
        movementPhysics.ResetGravity();
        movementPhysics.Stop();
        playerAudio?.PlayDeath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    // ----------- Power-ups de velocidad -----------

    public void ApplySpeedModifier(float multiplier, float duration)
    {
        if (speedModifierRoutine != null)
        {
            // Si ya hay un efecto activo, extender tiempo
            remainingDuration += duration;
            Debug.Log($"[SpeedModifier] Se extendio duracion, tiempo restante: {remainingDuration:F2}s");
        }
        else
        {
            // Nuevo efecto
            currentMultiplier = multiplier;
            remainingDuration = duration;

            MoveSpeed = baseSpeed * currentMultiplier;
            speedModifierRoutine = StartCoroutine(SpeedModifier());

            Debug.Log($"[SpeedModifier] Velocidad modificada: {MoveSpeed} (x{multiplier}) durante {duration}s");
        }
    }

    private IEnumerator SpeedModifier()
    {
        while (remainingDuration > 0)
        {
            remainingDuration -= Time.deltaTime;
            yield return null;
        }

        // Restaurar velocidad original
        MoveSpeed = baseSpeed;
        currentMultiplier = 1f;
        speedModifierRoutine = null;

        Debug.Log($"[SpeedModifier] Efecto terminado -> Velocidad restaurada a {MoveSpeed}");
    }

    // ========================
    //   Implementacion de IDamageable
    // ========================
    public int Life => health.Life;
    public bool IsAlive => health.IsAlive;

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        health.TakeDamage(damage, attackDirection);
        
    }
}
