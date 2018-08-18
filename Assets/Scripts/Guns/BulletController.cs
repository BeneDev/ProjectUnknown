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

    [SerializeField] Sprite bulletSprite;
    Sprite muzzleFlashSprite;

    [SerializeField] int muzzleFrameCount = 10;

    [SerializeField] float lifetime = 3f;
    float timeWhenShot;

    Vector3 velocity;

    SpriteRenderer rend;

	void Awake () {
        rend = GetComponent<SpriteRenderer>();
        muzzleFlashSprite = rend.sprite;
	}

    private void OnEnable()
    {
        StartCoroutine(ChangeToBulletSprite());
        timeWhenShot = Time.realtimeSinceStartup;
    }

    private void OnDisable()
    {
        rend.sprite = muzzleFlashSprite;
    }

    void Update () {
        velocity = new Vector3(transform.localScale.x * speed * Time.deltaTime, velocity.y, 0f);
        transform.position += velocity;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(rend.sprite != muzzleFlashSprite)
        {
            gameObject.SetActive(false);
            GameManager.Instance.GetBulletImpact(transform.position, transform.position - collision.transform.position);
        }
    }

    IEnumerator ChangeToBulletSprite()
    {
        for (int i = 0; i < muzzleFrameCount; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        rend.sprite = bulletSprite;
    }

    public void CalculateAccuracy(float chanceToMiss)
    {
        if(Random.value <= chanceToMiss)
        {
            velocity.y = Random.Range(-chanceToMiss * 0.25f, chanceToMiss * 0.25f);
        }
    }
}
