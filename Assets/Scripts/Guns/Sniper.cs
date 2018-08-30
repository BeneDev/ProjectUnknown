using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : GunManager {

    [SerializeField] LayerMask hitLayer;

    private void FixedUpdate()
    {
        if(owner)
        {
            RaycastHit2D redRayHit = Physics2D.Raycast(muzzle.transform.position, new Vector2(owner.transform.localScale.x, 0f), 50f, hitLayer);
            if(redRayHit.distance <= 0f)
            {
                Debug.DrawRay(muzzle.transform.position, new Vector3(owner.transform.localScale.x, 0f, 0f) * 50f, Color.red);
                //DrawLine(muzzle.transform.position, muzzle.transform.position + new Vector3(owner.transform.localScale.x, 0f, 0f) * 50f, Color.red, Time.fixedDeltaTime);
            }
            else
            {
                Debug.DrawRay(muzzle.transform.position, new Vector3(owner.transform.localScale.x, 0f, 0f) * redRayHit.distance, Color.red);
                //DrawLine(muzzle.transform.position, muzzle.transform.position + new Vector3(owner.transform.localScale.x, 0f, 0f) * redRayHit.distance, Color.red, 0.01f, Time.fixedDeltaTime);
            }
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float width, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(width, width);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    protected override void Shoot()
    {
        if (muzzle)
        {
            GameObject newBullet = GameManager.Instance.GetSniperBullet(muzzle.transform.position);
            if (Random.value > critChance)
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), damage, owner, knockbackStrength, knockbackDuration);
                GameManager.Instance.GetBulletTrail(newBullet);
            }
            else
            {
                newBullet.GetComponent<BulletController>().SetupBullet((float)(chanceToMiss / 100f), (int)((float)damage * critMultiplier), owner, knockbackStrength, knockbackDuration, true);
                GameManager.Instance.GetBulletTrail(newBullet);
            }
            newBullet.transform.localScale = owner.transform.localScale;
            StartCoroutine(ChangeSpriteToShooting());
        }
    }
}
