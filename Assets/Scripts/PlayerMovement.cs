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
    private bool onFloor;
    private bool atack;
    private bool isKnockback;

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
        if (health.IsAlive && !isKnockback)
        {
            movementPhysics.UpdateGroundState(longitudRaycast, floorLayer, coyoteTime);
            onFloor = movementPhysics.IsGrounded;

            if (!atack)
            {
                HandleMovement();
                HandleJump();
            }
            else
            {
                ApplyAttackBrake();
                ApplyJumpGravity();
            }

            if (Input.GetKeyUp(KeyCode.Z) && !atack && onFloor)
            {
                Atacking();
                
            }
        }

        animator.SetBool("onfloor", onFloor);
        animator.SetBool("atack", atack);
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
        atack = true;
        movementPhysics.ClearJumpBuffer();
        playerAudio?.PlaySwing();
    } 
    public void DeactivateAtacking() => atack = false;

    // ----------------- Danio y rebote -----------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!health.IsAlive || isKnockback) return;

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
                StartCoroutine(ApplyKnockback(attackDir, collision.collider));
            }
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction, Collider2D enemyCollider)
    {
        isKnockback = true;

        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, true);

        movementPhysics.ResetGravity();
        movementPhysics.Stop();

        Vector2 knockbackForce = new Vector2(direction.x * collitionForce, direction.y * (collitionForce * 0.5f));
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, false);

        movementPhysics.Stop();
        isKnockback = false;
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
