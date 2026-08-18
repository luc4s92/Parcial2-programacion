using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected Transform player;
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] protected float speed = 1.5f;
    [SerializeField] protected int life = 3;
    [SerializeField] private bool facesRightByDefault;

    [Header("Perception")]
    [SerializeField] private LayerMask sightObstructionLayers = 1 << 3;
    [SerializeField] private Transform sightOrigin;

    [Header("Damage reaction")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float hitRecoveryDuration = 0.6f;
    [SerializeField] private float bodyCleanupDelay = 1f;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyAudio enemyAudio;

    private StateMachine stateMachine;
    private EnemyHealth enemyHealth;
    private EnemyHitState hitState;
    private EnemyDeadState deadState;
    private Health playerHealth;
    private bool bodyDeletionRequested;

    private protected EnemyMovement Movement { get; private set; }
    private protected EnemyAnimationController AnimationController { get; private set; }
    private protected LayerMask SightObstructionLayers => sightObstructionLayers;
    private protected Transform SightOrigin => sightOrigin != null ? sightOrigin : transform;
    protected Transform Player => player;

    public bool IsAlive => enemyHealth == null || enemyHealth.IsAlive;
    public int Life => enemyHealth?.CurrentLife ?? life;

    protected virtual void Awake()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();

        ResolvePlayerReference();

        Movement = new EnemyMovement(rigidBody, transform, speed, facesRightByDefault);
        AnimationController = new EnemyAnimationController(animator);
        enemyHealth = new EnemyHealth(life);

        EnemyDamageReaction damageReaction = new EnemyDamageReaction(
            Movement,
            AnimationController,
            enemyAudio,
            knockbackForce
        );

        hitState = new EnemyHitState(
            damageReaction,
            hitRecoveryDuration,
            ResolveBehaviourState
        );
        deadState = new EnemyDeadState(damageReaction, HandleDeath);
        stateMachine = new StateMachine(CreateInitialState());
    }

    protected virtual void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.OnDeath += HandlePlayerDeath;
        }

        GameManager.Instance?.RegisterEnemy(this);
    }

    protected virtual void Update()
    {
        TickBehaviour(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.Tick();
    }

    protected virtual void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= HandlePlayerDeath;

        GameManager.Instance?.UnregisterEnemy(this);
    }

    public virtual bool TryTakeDamage(Vector2 sourcePosition, int totalDamage)
    {
        if (!IsAlive || IsInState(hitState) || IsInState(deadState))
            return false;

        enemyHealth.TakeDamage(totalDamage);
        if (!enemyHealth.IsAlive)
        {
            ChangeState(deadState);
            return true;
        }

        Vector2 knockbackDirection = new Vector2(
            transform.position.x - sourcePosition.x,
            0.1f
        ).normalized;

        hitState.Configure(knockbackDirection);
        ChangeState(hitState);
        return true;
    }

    public void DeactivateDamage()
    {
        if (IsInState(hitState))
            hitState.CompleteHit();
    }

    public void DeleteBody()
    {
        if (bodyDeletionRequested) return;

        bodyDeletionRequested = true;
        GameManager.Instance?.UnregisterEnemy(this);
        Destroy(gameObject);
    }

    public void NotifyPlayerDeath()
    {
        HandlePlayerDeath();
    }

    private protected void ChangeState(IState nextState)
    {
        stateMachine.ChangeState(nextState);
    }

    private protected bool IsInState(IState state)
    {
        return stateMachine.IsInState(state);
    }

    protected virtual void TickBehaviour(float deltaTime)
    {
    }

    protected virtual void HandleTriggerEnter(Collider2D collision)
    {
    }

    protected virtual void HandleCollisionEnter(Collision2D collision)
    {
    }

    protected virtual void HandleCollisionStay(Collision2D collision)
    {
    }

    protected virtual void HandleTargetDisabled()
    {
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private protected abstract IState CreateInitialState();
    protected abstract void ResolveBehaviourState();
    protected abstract void DropItem();

    private void HandleDeath()
    {
        GameManager.Instance?.EnemyKilled(this);
        DropItem();

        if (bodyCleanupDelay <= 0f)
            DeleteBody();
        else
            StartCoroutine(DeleteBodyAfterDelay());
    }

    private IEnumerator DeleteBodyAfterDelay()
    {
        yield return new WaitForSeconds(bodyCleanupDelay);
        DeleteBody();
    }

    private void HandlePlayerDeath()
    {
        HandleTargetDisabled();

        if (IsAlive && stateMachine != null && !IsInState(hitState))
            ResolveBehaviourState();
    }

    private void ResolvePlayerReference()
    {
        if (player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTriggerEnter(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollisionEnter(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollisionStay(collision);
    }
}
