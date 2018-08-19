using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : GunManager {

    protected override void Shoot()
    {
        if (muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetHeavyBullet(muzzle.transform.position);
            newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f), damage, owner);
            newBullet.transform.localScale = owner.transform.localScale;
            StartCoroutine(ChangeSpriteToShooting());
        }
    }
}
