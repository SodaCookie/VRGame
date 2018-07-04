using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon {
    private void Start()
    {
    }

    public override void GotHit(HitboxCollisionEventArgs other)
    {
        Shatter();
    }
}
