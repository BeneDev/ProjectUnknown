using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [SerializeField] float speed = 1f;

    [SerializeField] Sprite bulletSprite;

    [SerializeField] int muzzleFrameCount = 10;

    [SerializeField] float lifetime = 3f;
    float timeWhenShot;

    SpriteRenderer rend;

	void Awake () {
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeToBulletSprite());
        timeWhenShot = Time.realtimeSinceStartup;
	}
	
	void Update () {
        transform.position += new Vector3(transform.localScale.x * speed * Time.deltaTime, 0f, 0f);
        if(Time.realtimeSinceStartup > timeWhenShot + lifetime)
        {
            Destroy(gameObject);
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
}
