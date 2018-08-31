using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public float Lifetime
    {
        get
        {
            return lifetime;
        }
    }

    [SerializeField] float speed = 1f;

    int damage = 0;

    GameObject owner;

    [SerializeField] float lifetime = 3f;
    float timeWhenShot;

    bool isCritical = false;

    float knockbackDuration;
    float knockbackStrength;

    Vector3 velocity;

    int solidObjectsLayer;

    private void Awake()
    {
        solidObjectsLayer = LayerMask.NameToLayer("SolidObjects");
    }

    private void OnEnable()
    {
        timeWhenShot = Time.realtimeSinceStartup;
    }

    private void OnDisable()
    {
        damage = 0;
    }

    void Update () {
        velocity = new Vector3(transform.localScale.x * speed * Time.deltaTime, velocity.y, 0f);
        transform.position += velocity;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == owner) { return; }
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(damage, (collision.transform.position - transform.position).normalized * knockbackStrength, knockbackDuration, isCritical);
            if (isCritical)
            {
                GameManager.Instance.GetCritImpact(transform.position);
            }
            else
            {
                GameManager.Instance.GetBulletImpact(transform.position, transform.position - collision.transform.position, damage, true);
            }
        }
        else if(collision.gameObject.layer == solidObjectsLayer)
        {
            GameManager.Instance.GetBulletImpact(transform.position, transform.position - collision.transform.position, damage, false);
        }
        gameObject.SetActive(false);
    }

    public void SetupBullet(float chanceToMiss, int dmg, GameObject creator, float knockbackStr, float knockbackDur, bool isCrit = false)
    {
        owner = creator;
        damage = dmg;
        knockbackDuration = knockbackDur;
        knockbackStrength = knockbackStr;
        isCritical = isCrit;
        if(Random.value <= chanceToMiss)
        {
            velocity.y = Random.Range(-chanceToMiss * 0.25f, chanceToMiss * 0.1f);
        }
    }
}
