using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float longitudRaycast = 0.1f;
    [SerializeField] private float collitionForce = 10f;
    [SerializeField] LayerMask floorLayer;

    private bool onFloor;
    private bool takeDamage;
    private bool atack;
    private bool _isDead;
    public bool IsDead { get { return _isDead; } set { SetIsDead(value); } }
    public void SetIsDead (bool isDead) {  _isDead = isDead; }
    public int Life { get { return _life; } set { SetLife(value); } }
    public void SetLife(int life) { _life = life; }
    private float deathDelay = 4f;
    private Rigidbody2D rigidBody;
    [SerializeField] private int _life = 3;
    [SerializeField] private float playerSpeed;
    
    
    [SerializeField] private Vector3 posicion;
    [SerializeField] private Animator animator;
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
       
    }

    
    void Update()
    {
        if (!_isDead)
        {
            if (!atack)
            {
                Movement();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, floorLayer);
                onFloor = hit.collider != null;
               
                if (onFloor && Input.GetKeyDown(KeyCode.Space) && !takeDamage)
                {
                    rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                }
            }


            
            if (Input.GetKeyUp(KeyCode.Z) && !atack && onFloor)
            {
                Atacking();
            }

        }
            animator.SetBool("onfloor", onFloor);
            animator.SetBool("damage", takeDamage);
            animator.SetBool("atack", atack);
            animator.SetBool("death", _isDead);

        
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyWeapon") && !_isDead)
        {
            
            Vector2 attackDirection = new Vector2(
                transform.position.x - collision.transform.position.x,
                0.2f
            ).normalized;

            TakingDamage(attackDirection, 1); 
        }
    }
    public void TakingDamage(Vector2 direction, int totalDamage)
    {
        if (!takeDamage)
        {
            takeDamage = true;
            _life -= totalDamage;
            if(_life <= 0)
            {
                _isDead = true;
                Debug.Log("murido");
                StartCoroutine(LoadLoserScreenAfterDelay());
            }
            if (!_isDead)
            {
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.2f).normalized;
                rigidBody.AddForce(rebote * collitionForce, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator LoadLoserScreenAfterDelay()
    {
      
      
        yield return new WaitForSeconds(deathDelay);

        SceneManager.LoadScene("LoserScreen");
    }

    public void DeactivateDamage()
    {
        takeDamage = false;
        rigidBody.velocity = Vector2.zero;
       
    }

    public void Atacking()
    {
    atack= true;
    }

    public void DeactivateAtacking()
    {
        atack = false;
    }

    private void Movement()
    {
        float playerSpeedX = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;

        animator.SetFloat("movement", playerSpeedX * playerSpeed);

        if (playerSpeedX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (playerSpeedX > 0)
        {

            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 posicion = transform.position;

        if (!takeDamage)
        {
            transform.position = new Vector3(playerSpeedX + posicion.x, posicion.y, posicion.z);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
