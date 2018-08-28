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

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    [SerializeField] protected Sprite shootingSprite;
    [SerializeField] protected int framesToShowShootingSprite = 3;
    protected Sprite standardSprite;

    [SerializeField] protected float shotDelay = 0.1f;

    [SerializeField] protected int numberOfShots = 1;

    [SerializeField] protected float recoil = 1f;

    [SerializeField] protected int damage = 5;

    [Range(0, 1), SerializeField] protected float critChance = 0.05f;
    protected const float critMultiplier = 1.5f;

    [Range(0, 100), SerializeField] protected int chanceToMiss = 10;

    [SerializeField] protected BoxCollider2D triggerColl;

    [SerializeField] protected Transform muzzle;

    protected Rigidbody2D rb;

    protected SpriteRenderer rend;

    protected GameObject owner;

	protected virtual void Awake () {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        standardSprite = rend.sprite;
	}

    protected virtual void Shoot()
    {
        if(muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetRifleBullet(muzzle.transform.position);
            if(Random.value > critChance)
            {
                newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f), damage, owner);
            }
            else
            {
                newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f), (int)((float)damage * critMultiplier), owner);
            }
            newBullet.transform.localScale = owner.transform.localScale;
            StartCoroutine(ChangeSpriteToShooting());
        }
    }

    protected IEnumerator ChangeSpriteToShooting()
    {
        rend.sprite = shootingSprite;
        for (int i = 0; i < framesToShowShootingSprite; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        rend.sprite = standardSprite;
    }

    public virtual void Equip(GameObject ownedBy)
    {
        if(ownedBy.tag == "Player")
        {
            ownedBy.GetComponent<PlayerController>().OnShotFired += Shoot;
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        transform.localScale = ownedBy.transform.localScale;
        transform.localPosition = Vector3.zero;
        triggerColl.enabled = false;
        owner = ownedBy;
    }

    public void Unequip()
    {
        transform.parent = null;
        if(owner.tag == "Player")
        {
            owner.GetComponent<PlayerController>().OnShotFired -= Shoot;
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        triggerColl.enabled = true;
        owner = null;
    }
}
