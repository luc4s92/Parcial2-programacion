using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Trap : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask playerMask;
    private bool isGoingUp = false;
    private float delayTime= 1f;
    [SerializeField] private float speedUp;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }
    private void Update()
    {
        
        if (Physics2D.Raycast(transform.position, Vector3.down, distance, playerMask) && !isGoingUp)
        {
            Debug.Log("trampaa");
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

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
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 directionDamage = new Vector2(0, transform.position.y);
            PlayerMovement playerScript = collision.gameObject.GetComponent<PlayerMovement>();
           
           
            playerScript.TakingDamage(directionDamage, 1);
        }
        
      
       isGoingUp = true;
        StartCoroutine(DelayOnGround());
    }


    private IEnumerator DelayOnGround()
    {
        yield return new WaitForSeconds(delayTime);
     
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
}
