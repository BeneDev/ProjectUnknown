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

    public float KnockBackDuration
    {
        get
        {
            return knockbackDuration;
        }
    }
    
    [SerializeField] protected int framesToShowShootingSprite = 3;
    protected Sprite standardSprite;

    [SerializeField] protected float shotDelay = 0.1f;

    [SerializeField] protected int numberOfShots = 1;

    [SerializeField] protected float recoil = 1f;

    [SerializeField] protected int damage = 5;

    [SerializeField] protected float knockbackStrength = 10f;
    [SerializeField] protected float knockbackDuration = 0.5f;

    [Range(0, 1), SerializeField] protected float critChance = 0.05f;
    protected const float critMultiplier = 2f;

    [Range(0, 100), SerializeField] protected int chanceToMiss = 10;

    [SerializeField] protected BoxCollider2D triggerColl;

    [SerializeField] protected Transform muzzle;

    protected Rigidbody2D rb;

    protected SpriteRenderer rend;

    protected GameObject owner;

    int solidObjectsLayerNumber;

    GameObject ownLight;
    [SerializeField] float itemAppearRange = 15f;

    GameObject player;
    Vector3 toPlayer;

	protected virtual void Awake () {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        standardSprite = rend.sprite;
        solidObjectsLayerNumber = LayerMask.NameToLayer("SolidObjects");
        player = GameObject.FindGameObjectWithTag("Player");
	}

    protected void ManageItemLight()
    {
        toPlayer = player.transform.position - transform.position;
        if(toPlayer.magnitude > itemAppearRange && ownLight.activeSelf)
        {
            ownLight.SetActive(false);
        }
        else if(toPlayer.magnitude <= itemAppearRange && !ownLight.activeSelf)
        {
            ownLight.SetActive(true);
            ownLight.GetComponent<ParticleSystem>().Play();
        }
    }

    protected virtual void Shoot()
    {
        if(muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetRifleBullet(muzzle.transform.position);
            if(Random.value > critChance)
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), damage, owner, knockbackStrength, knockbackDuration);
            }
            else
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), (int)((float)damage * critMultiplier), owner, knockbackStrength, knockbackDuration, true);
            }
            newBullet.transform.localScale = owner.transform.localScale;
            GameManager.Instance.GetMuzzleFlash(muzzle.position);
        }
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
        GameManager.Instance.GiveItemLightBack(ownLight);
        CancelInvoke();
        ownLight = null;
    }

    public virtual void Unequip()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == solidObjectsLayerNumber && !owner && !ownLight)
        {
            ownLight = GameManager.Instance.GetItemLight(transform.position);
            InvokeRepeating("ManageItemLight", 0f, 1f);
        }
    }
}
