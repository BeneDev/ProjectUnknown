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

    Vector3 velocity;

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
            collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(damage, collision.transform.position - transform.position);
        }
        GameManager.Instance.GetBulletImpact(transform.position, transform.position - collision.transform.position, damage);
        gameObject.SetActive(false);
    }

    public void CalculateAccuracy(float chanceToMiss, int dmg, GameObject creator)
    {
        owner = creator;
        damage = dmg;
        if(Random.value <= chanceToMiss)
        {
            velocity.y = Random.Range(-chanceToMiss * 0.25f, chanceToMiss * 0.15f);
        }
    }
}
