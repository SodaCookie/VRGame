using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour {

    [SerializeField] private Hitbox hitbox;

    private void Start()
    {
        hitbox.OnHit += TakeDamage;
    }

    protected abstract void TakeDamage(HitboxCollisionEventArgs eventArgs);
}
