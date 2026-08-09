using UnityEngine;

public sealed class HellHoundEnemy : EnemyController
{
    [Header("Run")]
    [SerializeField] private float initialDirection = -1f;
    [SerializeField] private float maxLifetime = 12f;

    [Header("Contact damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float contactCooldown = 0.5f;

    private EnemyRun run;
    private EnemyRunState runState;
    private EnemyContactDamage contactDamageService;
    private float elapsedLifetime;

    private protected override IState CreateInitialState()
    {
        run = new EnemyRun(Movement, initialDirection);
        contactDamageService = new EnemyContactDamage(
            transform,
            contactDamage,
            contactCooldown
        );
        runState = new EnemyRunState(run, Movement, AnimationController);
        return runState;
    }

    protected override void TickBehaviour(float deltaTime)
    {
        contactDamageService.Tick(deltaTime);

        if (maxLifetime <= 0f) return;

        elapsedLifetime += deltaTime;
        if (elapsedLifetime >= maxLifetime)
            DeleteBody();
    }

    public void ConfigureRunDirection(float direction)
    {
        run.SetDirection(direction);
    }

    protected override void HandleCollisionEnter(Collision2D collision)
    {
        HandleCollision(collision, true);
    }

    protected override void HandleCollisionStay(Collision2D collision)
    {
        HandleCollision(collision, false);
    }

    private void HandleCollision(Collision2D collision, bool applyContactDamage)
    {
        if (!IsAlive) return;

        if (collision.collider.GetComponentInParent<EnemyController>() != null)
            return;

        if (collision.collider.GetComponentInParent<IDamageable>() != null)
        {
            if (applyContactDamage)
                contactDamageService.TryApply(collision.collider);

            return;
        }

        if (HasHorizontalContact(collision))
            DeleteBody();
    }

    protected override void ResolveBehaviourState()
    {
        ChangeState(runState);
    }

    protected override void DropItem()
    {
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        float direction = initialDirection < 0f ? -1f : 1f;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.right * direction * 2f
        );
    }

    private static bool HasHorizontalContact(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Mathf.Abs(collision.GetContact(i).normal.x) >= 0.75f)
                return true;
        }

        return false;
    }
}
