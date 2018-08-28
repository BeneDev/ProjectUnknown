using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamaginParticle : MonoBehaviour {

    int damage;
    float knockbackDuration;

    private void Awake()
    {
        GunManager gunMan = GetComponentInParent<GunManager>();
        damage = gunMan.Damage;
        knockbackDuration = gunMan.KnockBackDuration;
    }

    private void OnParticleCollision(GameObject other)
    {
        other.GetComponent<BaseEnemy>().TakeDamage(damage, other.transform.position - transform.position, knockbackDuration);
    }
}
