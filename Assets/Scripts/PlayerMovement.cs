using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float longitudRaycast = 0.1f;
    [SerializeField] private LayerMask floorLayer;

    [Header("Combate")]
    [SerializeField] private float collitionForce = 6f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float deathDelay = 4f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rigidBody;
    private Collider2D myCollider;
    private Health health;
    private bool onFloor;
    private bool atack;
    private bool isKnockback;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        health = GetComponent<Health>();

        health.OnLifeChanged += OnLifeChanged;
        health.OnDeath += OnDeath;
    }

    private void Start()
    {
        EventManager.TriggerPlayerLifeChanged(health.Life, health.Life);
    }

    private void Update()
    {
        if (health.IsAlive && !isKnockback)
        {
            if (!atack)
            {
                Movement();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, floorLayer);
                onFloor = hit.collider != null;

                if (onFloor && Input.GetKeyDown(KeyCode.Space))
                {
                    rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }

            if (Input.GetKeyUp(KeyCode.Z) && !atack && onFloor)
            {
                Atacking();
            }
        }

        animator.SetBool("onfloor", onFloor);
        animator.SetBool("atack", atack);
    }

    private void Movement()
    {
        float inputX = Input.GetAxis("Horizontal");
        rigidBody.velocity = new Vector2(inputX * playerSpeed, rigidBody.velocity.y);

        animator.SetFloat("movement", Mathf.Abs(inputX * playerSpeed));

        if (inputX < 0) transform.localScale = new Vector3(-1, 1, 1);
        if (inputX > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    public void Atacking() => atack = true;
    public void DeactivateAtacking() => atack = false;

    // ----------------- Daño y rebote -----------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!health.IsAlive || isKnockback) return;

        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("[PlayerMovement] Colisión con Enemy");

            Vector2 attackDir = new Vector2(
                transform.position.x - collision.transform.position.x,
                0.5f
            ).normalized;

            health.TakeDamage(1, attackDir);

            StartCoroutine(ApplyKnockback(attackDir, collision.collider));
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction, Collider2D enemyCollider)
    {
        isKnockback = true;

        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, true);

        rigidBody.velocity = Vector2.zero;

        Vector2 knockbackForce = new Vector2(direction.x * collitionForce, direction.y * (collitionForce * 0.5f));
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        if (enemyCollider != null)
            Physics2D.IgnoreCollision(myCollider, enemyCollider, false);

        rigidBody.velocity = Vector2.zero;
        isKnockback = false;
    }

    private void OnLifeChanged(int currentLife, int maxLife, Vector2 attackDirection)
    {
        EventManager.TriggerPlayerLifeChanged(currentLife, maxLife);

        if (!health.IsAlive) return;

        animator.SetBool("damage", true);
        StartCoroutine(ResetDamageFlag());
    }

    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("damage", false);
    }

    private void OnDeath()
    {
        animator.SetBool("death", true);
        rigidBody.velocity = Vector2.zero;
        StartCoroutine(LoadLoserScreenAfterDelay());
    }

    private IEnumerator LoadLoserScreenAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        SceneManager.LoadScene("LoserScreen");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
