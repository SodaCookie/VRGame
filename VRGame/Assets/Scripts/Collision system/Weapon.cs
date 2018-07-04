using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    protected void Shatter()
    {
        Destroy(gameObject);
    }

    public abstract void GotHit(HitboxCollisionEventArgs other);
}
