using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour {

    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private bool isWeapon;

    private void Awake()
    {
        gameObject.tag = "Hitbox";
        associatedWeapon = GetComponent<Weapon>();
        if(associatedWeapon != null)
        {
            OnHit += associatedWeapon.GotHit;
        }
    }

    public Type hitboxType;
    public event Action<HitboxCollisionEventArgs> OnHit;

    private Weapon associatedWeapon; // Either the weapon this hitbox is associated with, or null if there is none

    private void RegisterHit(Weapon attacker)
    {
        OnHit(new HitboxCollisionEventArgs(attacker));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hitbox collidedHitbox = collision.gameObject.GetComponent<Hitbox>();
        if (collidedHitbox == null) return;

        if(isPlayer != collidedHitbox.isPlayer)
        {
            if (isWeapon)
            {
                // They're both weapons, they hit each other
                if (collidedHitbox.isWeapon)
                    RegisterHit(collidedHitbox.associatedWeapon);
                // Only one is a weapon, the one that isn't doesn't get hit
                collidedHitbox.RegisterHit(associatedWeapon);
            }
            else
            {
                if (!collidedHitbox.isWeapon)
                {
                    // Neither is a weapon - they hit each other
                    RegisterHit(null);
                    collidedHitbox.RegisterHit(null);
                }
            }
        }
    }


}
