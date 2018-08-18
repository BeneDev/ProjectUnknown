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

    [SerializeField] protected float shotDelay = 0.1f;

    [SerializeField] protected int numberOfShots = 1;

    [SerializeField] protected float recoil = 1f;

    [Range(0, 100), SerializeField] protected int chanceToMiss = 10; 

    [SerializeField] protected GameObject bullet;

    [SerializeField] protected BoxCollider2D triggerColl;

    [SerializeField] protected Transform muzzle;

    protected Rigidbody2D rb;

    protected PlayerController owner;

	protected virtual void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

    protected virtual void Shoot()
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
