using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

    public float ShotDelay
    {
        get
        {
            return shotDelay;
        }
    }

    [SerializeField] float shotDelay = 0.1f;

    [SerializeField] GameObject bullet;

    [SerializeField] BoxCollider2D triggerColl;

    Rigidbody2D rb;

	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		
	}

    void Shoot()
    {
        print("POW!");
    }

    public void Equip(PlayerController player)
    {
        player.OnShotFired += Shoot;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        transform.localScale = player.transform.localScale;
        triggerColl.enabled = false;

    }

    public void Unequip(PlayerController player)
    {
        transform.parent = null;
        player.OnShotFired -= Shoot;
        rb.bodyType = RigidbodyType2D.Dynamic;
        triggerColl.enabled = true;
    }
}
