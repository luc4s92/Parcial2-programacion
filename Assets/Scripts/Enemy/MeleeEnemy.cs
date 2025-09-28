using UnityEngine;
using System.Collections.Generic;

public class MeleeEnemy : EnemyController
{
   

    [Header("Drops")]
    [SerializeField] private List<GameObject> possibleDrops; // lista de prefabs de items
    [SerializeField] private float dropChance = 1f; // % probabilidad de soltar algo

    protected override void Start()
    {
        base.Start();
        strategy = new ChaseStrategy();
    }

    protected override void EnemyBehaviour()
    {
        if (isDead || Player == null || !isPlayerAlive)
        {
            Rigidbody.velocity = Vector2.zero;
            animator.SetBool("onMovement", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (distanceToPlayer < DetectionRadius && !IsTakingDamage)
        {
            strategy.Execute(this);
            animator.SetBool("onMovement", true);

            Vector2 dir = (Player.position - transform.position).normalized;
            transform.localScale = dir.x < 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
        else
        {
            animator.SetBool("onMovement", false);
            Rigidbody.velocity = Vector2.zero;
        }
    }

    protected override void DropItem()
    {
        if (possibleDrops.Count == 0) return;

        // chequeo si dropea algo
        if (Random.value <= dropChance)
        {
            int index = Random.Range(0, possibleDrops.Count);
            GameObject itemToDrop = possibleDrops[index];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);

            Debug.Log($"[{gameObject.name}] soltó: {itemToDrop.name}");
        }
    }
}
