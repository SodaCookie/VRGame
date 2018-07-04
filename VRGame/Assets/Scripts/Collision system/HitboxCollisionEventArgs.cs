using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollisionEventArgs : MonoBehaviour {
    private Weapon attacker;

    public HitboxCollisionEventArgs(Weapon attacker)
    {
        this.attacker = attacker;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
