using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSplatter : GunManager {

    protected override void Shoot()
    {
        if (muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetHeavyBullet(muzzle.transform.position);
            if (Random.value > critChance)
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), damage, owner, knockbackStrength, knockbackDuration);
            }
            else
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), (int)((float)damage * critMultiplier), owner, knockbackStrength, knockbackDuration, true);
            }
            newBullet.transform.localScale = owner.transform.localScale;
            GameManager.Instance.GetMuzzleFlash(muzzle.position);
        }
    }
}
