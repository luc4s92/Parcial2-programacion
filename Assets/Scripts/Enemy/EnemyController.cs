using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] float detectionRadius = 5.0f;
    [SerializeField] float speed = 4.0f;
    [SerializeField] int life = 3;

    private bool takeDamage;
    private Rigidbody2D rigidBody;
    private float collitionForce = 3f;
    private Vector2 movement;
    private bool onMovement;
    private bool isDead;
    private bool isPlayerAlive;
    private Animator animator;
    void Start()
    {
        isPlayerAlive = true;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (isPlayerAlive && !isDead)
        {
            Movement();

        }





        animator.SetBool("damage", takeDamage);
        animator.SetBool("death", isDead);
        animator.SetBool("onMovement", onMovement);

    }

    public void DeactivateDamage()
    {
        takeDamage = false;
        rigidBody.velocity = Vector2.zero;

    }
    public void TakingDamage(Vector2 direction, int totalDamage)
    {
        if (!takeDamage)
        {
            life -= totalDamage;
            takeDamage = true;
            if (life <= 0)
            {
                isDead = true;
                onMovement = false;
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.1f).normalized;
                rigidBody.AddForce(rebote * collitionForce, ForceMode2D.Impulse);
            }


        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 directionDamage = new Vector2(transform.position.x, 0);
            PlayerMovement playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            playerScript.TakingDamage(directionDamage, 1);
            isPlayerAlive = !playerScript.IsDead;
            if (!isPlayerAlive)
            {
                onMovement = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
            TakingDamage(directionDamage, 1);
        }
    }
    private void Movement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (direction.x > 0)
            {

                transform.localScale = new Vector3(-1, 1, 1);
            }
            movement = new Vector2(direction.x, 0);
            onMovement = true;
        }
        else
        {
            movement = Vector2.zero;
            onMovement = false;
        }

        if (!takeDamage)
            rigidBody.MovePosition(rigidBody.position + movement * speed * Time.deltaTime);
    }

    public void DeleteDody()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}