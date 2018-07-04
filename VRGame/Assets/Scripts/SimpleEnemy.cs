using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : BaseEnemy {

    protected override void TakeDamage(HitboxCollisionEventArgs eventArgs)
    {
        Destroy(gameObject);
    }

}
