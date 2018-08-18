using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    [SerializeField] protected int maxHealth = 5;
    protected int health;

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage, Vector3 knockBackDir)
    {
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
