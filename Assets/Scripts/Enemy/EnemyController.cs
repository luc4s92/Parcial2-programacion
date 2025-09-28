using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IEnemyContext
{
    [Header("Stats")]
    [SerializeField] protected Transform player;
    [SerializeField] protected float detectionRadius = 5.0f;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected int life = 3;

    [Header("State")]
    protected bool takeDamage;
    protected bool isDead;
    protected bool isPlayerAlive = true;

    [Header("Components")]
    protected Rigidbody2D rigidBody;
    [SerializeField] protected Animator animator;
    protected SpriteRenderer sprite;

    // Estrategia de movimiento
    protected IEnemyStrategy strategy;

    // Exposición para Strategy
    public Transform Transform => transform;
    public Rigidbody2D Rigidbody => rigidBody;
    public float Speed => speed;
    public Transform Player => player;
    public float DetectionRadius => detectionRadius;
    public bool IsTakingDamage => takeDamage;
    public bool IsAlive => !isDead;
    public bool IsPlayerAlive => isPlayerAlive;

    // ---------------- UNITY ----------------
    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.OnDeath += HandlePlayerDeath;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterEnemy(this);
    }

    protected virtual void Update()
    {
        animator.SetBool("damage", takeDamage);
        animator.SetBool("death", isDead);
    }

    protected virtual void FixedUpdate()
    {
        if (isDead || !isPlayerAlive)
        {
            if (rigidBody != null)
                rigidBody.velocity = Vector2.zero;

            animator.SetBool("onMovement", false);
            return;
        }

        EnemyBehaviour();
    }

    // ---------------- DAMAGE ----------------
    public virtual void TakingDamage(Vector2 direction, int totalDamage)
    {
        if (!takeDamage && !isDead)
        {
            life -= totalDamage;
            takeDamage = true;

            if (life <= 0)
            {
                isDead = true;
                rigidBody.velocity = Vector2.zero;

                Debug.Log($"[{gameObject.name}] Muerto → notificando al GameManager");

                // Avisar al GameManager
                if (GameManager.Instance != null)
                    GameManager.Instance.EnemyKilled(this);

                // 🔹 Drop de item al morir
                DropItem();
            }
            else
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.1f).normalized;
                rigidBody.AddForce(rebound * 3f, ForceMode2D.Impulse);
            }
        }
    }

    public void DeactivateDamage()
    {
        takeDamage = false;
        if (rigidBody != null)
            rigidBody.velocity = Vector2.zero;
    }

    public void DeleteBody()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterEnemy(this);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") && !isDead)
        {
            TakingDamage(new Vector2(collision.transform.position.x, 0), 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void HandlePlayerDeath()
    {
        isPlayerAlive = false;
        if (rigidBody != null)
            rigidBody.velocity = Vector2.zero;
    }

    public void NotifyPlayerDeath()
    {
        isPlayerAlive = false;
        if (rigidBody != null)
            rigidBody.velocity = Vector2.zero;
    }

    // ---------------- ABSTRACT ----------------
    protected abstract void EnemyBehaviour();

    // 🔹 Nuevo método abstracto: cada enemigo decide qué item suelta
    protected abstract void DropItem();
}
