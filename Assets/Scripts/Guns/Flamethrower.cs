using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : GunManager {

    [SerializeField] ParticleSystem flames;
    ParticleSystem.EmissionModule flamesEmission;

    PlayerController ownerController;

    //[SerializeField] float timeBetweenHits = 1f;
    //float timeWhenLastHit = 0f;

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
            //TODO make flames go faster or slower depending on the player speed
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
}
