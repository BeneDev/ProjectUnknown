using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    [SerializeField] protected int maxHealth = 5;
    protected int health;
    [SerializeField] int attack = 3;
    [SerializeField] float knockBackStrength = 1f;
    [SerializeField] float knockBackDuration = 0.2f;

    [SerializeField] float flashDuration = 0.1f;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    [SerializeField] Color flashUpColor;

    SpriteRenderer rend;

    protected virtual void Awake()
    {
        health = maxHealth;
        rend = GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
    }

    public void TakeDamage(int damage, Vector3 knockBackDir)
    {
        StartCoroutine(SetBackToDefaultShader(flashDuration));
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Make the Sprite show the normal colors again after a set amount of time
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    IEnumerator SetBackToDefaultShader(float sec)
    {
        // Let the enemy sprite flash up white
        rend.material.shader = shaderGUItext;
        rend.color = flashUpColor;
        // Wait
        yield return new WaitForSeconds(sec);
        // Set the enemy sprite to normal color again
        rend.material.shader = shaderSpritesDefault;
        rend.color = Color.white;
    }

    protected virtual void Die()
    {
        //player.GainExp(expToGive);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().TakeDamage(attack, (collision.transform.position - transform.position).normalized * knockBackStrength, knockBackDuration);
        }
    }
}
