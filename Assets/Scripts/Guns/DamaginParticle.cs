using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamaginParticle : MonoBehaviour {

    int damage;

    private void Awake()
    {
        damage = GetComponentInParent<GunManager>().Damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        other.GetComponent<BaseEnemy>().TakeDamage(damage, other.transform.position - transform.position);
    }
}
