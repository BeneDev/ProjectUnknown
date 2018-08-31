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
        flames.gameObject.SetActive(false);
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
                StartCoroutine(DisableFlamesAfterSeconds(flames.main.startLifetime.constantMax));
            }
        }
    }

    IEnumerator DisableFlamesAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if(ownerController)
        {
            if (!ownerController.IsShooting)
            {
                flames.gameObject.SetActive(false);
            }
        }
    }

    protected override void Shoot()
    {
        if (flames)
        {
            flames.gameObject.SetActive(true);
            if(!flamesEmission.enabled)
            {
                flamesEmission.enabled = true;
            }
        }
    }
}
