using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [SerializeField] float speed = 1f;

    [SerializeField] Sprite bulletSprite;

    [SerializeField] int muzzleFrameCount = 10;

    [SerializeField] float lifetime = 3f;
    float timeWhenShot;

    Vector3 velocity;

    SpriteRenderer rend;

	void Awake () {
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeToBulletSprite());
        timeWhenShot = Time.realtimeSinceStartup;
	}
	
	void Update () {
        velocity = new Vector3(transform.localScale.x * speed * Time.deltaTime, velocity.y, 0f);
        if(Time.realtimeSinceStartup > timeWhenShot + lifetime)
        {
            Destroy(gameObject);
        }
        transform.position += velocity;
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
            velocity.y = Random.Range(-chanceToMiss * 0.5f, chanceToMiss * 0.5f);
        }
    }
}
