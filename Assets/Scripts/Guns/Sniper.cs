using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : GunManager {

    protected override void Shoot()
    {
        if (bullet && muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetSniperBullet(muzzle.transform.position);
            newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f), damage, owner);
            newBullet.transform.localScale = owner.transform.localScale;
            StartCoroutine(ChangeSpriteToShooting());
        }
    }
}
