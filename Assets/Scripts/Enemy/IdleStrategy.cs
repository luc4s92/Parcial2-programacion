using UnityEngine;

public class IdleStrategy : IEnemyStrategy
{
    public void Execute(IEnemyContext context)
    {
        // El enemigo no hace nada
        context.Rigidbody.velocity = Vector2.zero;
    }
}
