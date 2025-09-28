//using UnityEngine;

//public class RangedEnemy : EnemyController
//{
//    [SerializeField] private GameObject projectilePrefab;
//    [SerializeField] private Transform firePoint;
//    [SerializeField] private float fireRate = 2f;
//    private float fireTimer;

//    protected override void EnemyBehaviour()
//    {
//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        if (distanceToPlayer < detectionRadius)
//        {
//            fireTimer += Time.deltaTime;
//            if (fireTimer >= fireRate)
//            {
//                ShootProjectile();
//                fireTimer = 0;
//            }
//        }
//    }

//    private void ShootProjectile()
//    {
//        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
//        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
//        Vector2 direction = (player.position - firePoint.position).normalized;
//        rb.AddForce(direction * 5f, ForceMode2D.Impulse);
//    }
//}
