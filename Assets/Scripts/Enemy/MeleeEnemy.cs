using UnityEngine;

public class MeleeEnemy : EnemyController
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    protected override void Start()
    {
        base.Start();
        strategy = new ChaseStrategy(); // asignamos estrategia de persecución
    }

    protected override void EnemyBehaviour()
    {
        if (Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (distanceToPlayer < DetectionRadius && !IsTakingDamage)
        {
            strategy.Execute(this); // ejecutar estrategia
            animator.SetBool("onMovement", true);

            // Flip horizontal
            Vector2 dir = (Player.position - transform.position).normalized;
            if (dir.x < 0) transform.localScale = new Vector3(1, 1, 1);
            else if (dir.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            animator.SetBool("onMovement", false);
            Rigidbody.velocity = Vector2.zero;
        }
    }
}
