using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : GunManager {

    [SerializeField] ParticleSystem flames;
    ParticleSystem.EmissionModule flamesEmission;

    [SerializeField] BoxCollider2D damagingTriggerVolume;

    PlayerController ownerController;

    [SerializeField] float timeBetweenHits = 1f;
    float timeWhenLastHit = 0f;

    protected override void Awake()
    {
        base.Awake();
        flamesEmission = flames.emission;
        flamesEmission.enabled = false;
    }

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
        if(owner)
        {
            flames.gameObject.transform.localScale = owner.transform.localScale;
        }
        if (!flamesEmission.enabled) { return; }
        if(ownerController)
        {
            if (!ownerController.IsShooting)
            {
                flamesEmission.enabled = false;
            }
        }
    }

    protected override void Shoot()
    {
        if (flames)
        {
            if(!flamesEmission.enabled)
            {
                flamesEmission.enabled = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (flames)
        {
            if (flamesEmission.enabled)
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
