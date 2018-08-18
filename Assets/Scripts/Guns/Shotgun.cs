using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunManager {

    protected override void Shoot()
    {
        if (bullet && muzzle)
        {
            for (int i = 0; i < numberOfShots; i++)
            {
                GameObject newBullet = GameManager.Instance.GetShotgunBullet(muzzle.transform.position);
                newBullet.GetComponent<BulletController>().CalculateAccuracy((float)(chanceToMiss / 100f));
                newBullet.transform.localScale = owner.transform.localScale;
            }
        }
    }
}
