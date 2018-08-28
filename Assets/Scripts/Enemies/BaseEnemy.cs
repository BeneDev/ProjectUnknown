using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseEnemy : MonoBehaviour {

    [SerializeField] protected int maxHealth = 5;
    protected int health;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected int attack = 3;
    [SerializeField] protected float knockBackStrength = 1f;
    [SerializeField] protected float knockBackDuration = 0.2f;

    [SerializeField] float flashDuration = 0.1f;
    protected Shader shaderGUItext;
    protected Shader shaderSpritesDefault;

    [SerializeField] protected Color flashUpColor;

    protected SpriteRenderer rend;

    protected Rigidbody2D rb;

    [SerializeField] protected LayerMask collidingLayer;
    [SerializeField] protected BoxCollider2D coll;

    protected enum EnemyState
    {
        patroling,
        foundPlayer,
        knockedBack,
        searchPlayer,
        suspicious
    }
    protected EnemyState state = EnemyState.patroling;

    protected struct EnemyRaycasts // To store the informations of raycasts around the player to calculate physics
    {
        public RaycastHit2D left;
        public RaycastHit2D right;
    }
    protected EnemyRaycasts raycasts;

    protected virtual void Awake()
    {
        health = maxHealth;
        rend = GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        UpdateRaycasts();
        if(state == EnemyState.patroling)
        {
            MoveAround();
        }
    }

    protected virtual void MoveAround()
    {
        if(transform.localScale.x < 0f && raycasts.right)
        {
            print("turn left");
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if(transform.localScale.x > 0f && raycasts.left)
        {
            print("turn right");
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        transform.position += new Vector3(-transform.localScale.x * speed * Time.deltaTime, rb.velocity.y);
    }

    void UpdateRaycasts()
    {
        raycasts.left = Physics2D.Raycast((Vector2)coll.bounds.center + new Vector2(-coll.bounds.extents.x, -coll.bounds.extents.y * 0.5f), Vector2.left, 0.5f, collidingLayer);
        raycasts.right = Physics2D.Raycast((Vector2)coll.bounds.center + new Vector2(coll.bounds.extents.x, -coll.bounds.extents.y * 0.5f), Vector2.right, 0.5f, collidingLayer);    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay((Vector2)coll.bounds.center + new Vector2(-coll.bounds.extents.x, -coll.bounds.extents.y * 0.5f), Vector2.left * 0.5f);
    //    Debug.DrawRay((Vector2)coll.bounds.center + new Vector2(coll.bounds.extents.x, -coll.bounds.extents.y * 0.5f), Vector2.right * 0.5f);
    //}

    protected virtual void MoveTowards(Vector3 targetPos)
    {

    }

    public void TakeDamage(int damage, Vector2 knockback, float knockedBackDur)
    {
        StartCoroutine(SetBackToDefaultShader(flashDuration));
        health -= damage;
        rb.velocity = new Vector2(knockback.x, 0f) * Time.deltaTime;
        StartCoroutine(GetKnockedBackForSeconds(knockedBackDur));
        if(health <= 0)
        {
            Die();
        }
    }

    IEnumerator GetKnockedBackForSeconds(float seconds)
    {
        state = EnemyState.knockedBack;
        yield return new WaitForSeconds(seconds);
        state = EnemyState.patroling;
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
