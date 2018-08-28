using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSplatter : GunManager {

    protected override void Shoot()
    {
        if (muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetHeavyBullet(muzzle.transform.position);
            newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), damage, owner, knockbackStrength, knockbackDuration);
            newBullet.transform.localScale = owner.transform.localScale;
            StartCoroutine(ChangeSpriteToShooting());
        }
    }
}
