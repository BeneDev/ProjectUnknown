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

    public float Recoil
    {
        get
        {
            return recoil;
        }
    }

    [SerializeField] float shotDelay = 0.1f;

    [SerializeField] float recoil = 1f;

    [Range(0, 100), SerializeField] int chanceToMiss = 10; 

    [SerializeField] GameObject bullet;

    [SerializeField] BoxCollider2D triggerColl;

    [SerializeField] Transform muzzle;

    Rigidbody2D rb;

    PlayerController owner;

	void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

    void Shoot()
    {
        if(bullet && muzzle)
        {
            GameObject newBullet = Instantiate(bullet, muzzle.position, transform.rotation);
            newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f));
            newBullet.transform.localScale = owner.transform.localScale;
        }
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
        owner = player;
    }

    public void Unequip()
    {
        transform.parent = null;
        owner.OnShotFired -= Shoot;
        rb.bodyType = RigidbodyType2D.Dynamic;
        triggerColl.enabled = true;
        owner = null;
    }
}
