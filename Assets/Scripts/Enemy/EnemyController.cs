using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [SerializeField] protected Transform player;
    [SerializeField] protected float detectionRadius = 5.0f;
    [SerializeField] protected float speed = 4.0f;
    [SerializeField] protected int life = 3;

    protected bool takeDamage;
    protected bool isDead;
    protected bool isPlayerAlive;
    protected Rigidbody2D rigidBody;
    protected Animator animator;

    protected virtual void Start()
    {
        isPlayerAlive = true;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (isPlayerAlive && !isDead)
        {
            EnemyBehaviour();
        }

        animator.SetBool("damage", takeDamage);
        animator.SetBool("death", isDead);
    }

    public virtual void TakingDamage(Vector2 direction, int totalDamage)
    {
        if (!takeDamage)
        {
            life -= totalDamage;
            takeDamage = true;

            if (life <= 0)
            {
                isDead = true;
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
        rigidBody.velocity = Vector2.zero;
    }

    public void DeleteBody()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            playerScript.TakingDamage(new Vector2(transform.position.x, 0), 1);
            isPlayerAlive = !playerScript.IsDead;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            TakingDamage(new Vector2(collision.transform.position.x, 0), 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    //  Cada tipo de enemigo implementará su propia lógica
    protected abstract void EnemyBehaviour();
}
