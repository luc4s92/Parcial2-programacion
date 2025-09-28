using UnityEngine;

public class PatrolStrategy : IEnemyStrategy
{
    private float direction = 1f;
    private float patrolRange = 3f;
    private Vector3 startPos;

    public PatrolStrategy(Vector3 startPosition)
    {
        startPos = startPosition;
    }

    public void Execute(IEnemyContext context)
    {
        // Movimiento simple de izquierda a derecha
        context.Rigidbody.velocity = new Vector2(direction * context.Speed, context.Rigidbody.velocity.y);

        // Cambiar dirección si se pasa del rango
        if (Mathf.Abs(context.Transform.position.x - startPos.x) > patrolRange)
        {
            direction *= -1;
        }
    }
}
