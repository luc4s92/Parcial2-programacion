using UnityEngine;

public class ChaseStrategy : IEnemyStrategy
{
    public void Execute(IEnemyContext context)
    {
       
        if (context.Player == null) return;

        Vector2 direction = (context.Player.position - context.Transform.position).normalized;

        // Ahora si: velocidad en unidades/segundo
        Vector2 newVelocity = new Vector2(
            direction.x * context.Speed,
            context.Rigidbody.linearVelocity.y
        );

        context.Rigidbody.linearVelocity = newVelocity;

    }
}
