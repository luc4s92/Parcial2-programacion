using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float distance = 2f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float speedUp = 3f;
    [SerializeField] private float delayTime = 1f;

    private Vector3 originalPosition;
    private bool isGoingUp = false;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Detectar jugador debajo
        if (!isGoingUp && Physics2D.Raycast(transform.position, Vector2.down, distance, playerMask))
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        // Si ya volvió a su posición original, dejar de subir
        if (Vector2.Distance(transform.position, originalPosition) < 0.01f)
        {
            isGoingUp = false;
            rb2d.constraints = RigidbodyConstraints2D.None;
        }
    }

    private void FixedUpdate()
    {
        if (isGoingUp)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPosition, speedUp * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Congelar la trampa al golpear
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;

        // Aplicar daño si colisiona con algo que implemente IDamageable
        IDamageable playerScript = collision.gameObject.GetComponent<IDamageable>();
        if (playerScript != null && playerScript.IsAlive)
        {
            // Dirección del daño: desde la trampa hacia el jugador
            Vector2 directionDamage = (collision.transform.position - transform.position).normalized;
            playerScript.TakeDamage(1, directionDamage);
        }

        // Subir después de golpe
        isGoingUp = true;
        StartCoroutine(DelayBeforeUp());
    }

    private IEnumerator DelayBeforeUp()
    {
        yield return new WaitForSeconds(delayTime);
        // Ya dejamos que FixedUpdate se encargue de mover hacia arriba
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
}
