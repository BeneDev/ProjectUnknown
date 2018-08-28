using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    [SerializeField] protected int maxHealth = 5;
    protected int health;

    //[SerializeField] float freezeFrameDuration = 0.1f;

    [SerializeField] float flashDuration = 0.1f;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    [SerializeField] Color flashUpColor;

    SpriteRenderer rend;

    //bool isFreezing;

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
        //if(!isFreezing)
        //{
        //    isFreezing = true;
        //    FreezeFrames(freezeFrameDuration);
        //}
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

    //void FreezeFrames(float seconds)
    //{
    //    Time.timeScale = 0f;
    //    float freezeEndTime = Time.realtimeSinceStartup + seconds;
    //    while (Time.realtimeSinceStartup < freezeEndTime)
    //    {
    //        // Do nothing
    //    }
    //    Time.timeScale = 1f;
    //}

    protected virtual void Die()
    {
        //player.GainExp(expToGive);
        Destroy(gameObject);
    }
}
