using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : GunManager {

    [SerializeField] ParticleSystem flames;

    [SerializeField] BoxCollider2D damagingTriggerVolume;

    PlayerController ownerController;

    [SerializeField] float timeBetweenHits = 1f;
    float timeWhenLastHit = 0f;

    public override void Equip(GameObject ownedBy)
    {
        base.Equip(ownedBy);
        if (owner.GetComponent<PlayerController>())
        {
            ownerController = owner.GetComponent<PlayerController>();
        }
    }

    protected void Update()
    {
        if (!flames.isPlaying) { return; }
        if(ownerController)
        {
            if (!ownerController.IsShooting)
            {
                flames.Stop();
            }
        }
    }

    protected override void Shoot()
    {
        if (flames)
        {
            if(!flames.isPlaying)
            {
                flames.Play();
            }
            //GameObject newBullet = GameManager.Instance.GetHeavyBullet(muzzle.transform.position);
            //newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f), damage, owner);
            //newBullet.transform.localScale = owner.transform.localScale;
            //StartCoroutine(ChangeSpriteToShooting());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (flames)
        {
            if (flames.isPlaying)
            {
                if (collision.gameObject.tag == "Enemy" && Time.realtimeSinceStartup > timeWhenLastHit + timeBetweenHits)
                {
                    collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(damage, collision.transform.position - transform.position);
                    timeWhenLastHit = Time.realtimeSinceStartup;
                }
            }
        }
    }
}
