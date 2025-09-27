using UnityEngine;

public class MeleeEnemy : EnemyController
{
    [Header("Animation")]
    [SerializeField] private Animator animator; 

    protected override void EnemyBehaviour()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Girar al enemigo seg�n la direcci�n
            if (direction.x < 0) transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x > 0) transform.localScale = new Vector3(-1, 1, 1);

            // Movimiento
            if (!takeDamage)
            {
                rigidBody.MovePosition(rigidBody.position + direction * speed * Time.deltaTime);
                animator.SetBool("onMovement", true); // Activamos animaci�n de caminar
            }
        }
        else
        {
            animator.SetBool("onMovement", false); // Detener animaci�n si no detecta al jugador
        }
    }
}
