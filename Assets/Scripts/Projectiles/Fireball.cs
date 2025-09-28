using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    // Start is called before the first frame update
    public override void OnHit(IDamageable target)
    {
        target.TakeDamage(2, Vector2.zero);
        Destroy(gameObject);
    }
}
