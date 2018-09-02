using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseEnemy : MonoBehaviour {

    [SerializeField] protected int maxHealth = 5;
    protected int health;
    [SerializeField] protected float maxSpeed = 5f;
    protected float speed;
    [Range(0, 1), SerializeField] protected float patrolSpeedMultiplier = 0.75f;
    [SerializeField] protected int attack = 3;
    [SerializeField] protected float knockBackStrength = 1f;
    [SerializeField] protected float knockBackDuration = 0.2f;

    [SerializeField] protected float maxSpeedStamina = 5f;
    [SerializeField] protected float exhaustionTime = 1.5f;
    protected float exhaustionCounter = 0f;

    [SerializeField] float flashDuration = 0.1f;
    protected Shader shaderGUItext;
    protected Shader shaderSpritesDefault;
    protected Shader normalShader;

    [SerializeField] protected Color flashUpColor;

    protected SpriteRenderer rend;

    protected Rigidbody2D rb;

    [SerializeField] protected LayerMask collidingLayer;
    [SerializeField] protected BoxCollider2D coll;

    protected PlayerController player;
    protected Vector3 toPlayer;

    [SerializeField] protected LayerMask playerAndObjectsLayer;

    protected float timeWhenLastSawPlayer;
    [SerializeField] protected float timeToSearchForPlayer = 2f;
    protected Vector3 lastPosPlayerSeen;

    [SerializeField] protected float sightReach = 15f;

    protected enum EnemyState
    {
        patroling,
        foundPlayer,
        knockedBack,
        searchPlayer,
        suspicious,
        exhausted
    }
    protected EnemyState state = EnemyState.patroling;

    protected struct EnemyRaycasts // To store the informations of raycasts around the player to calculate physics
    {
        public RaycastHit2D left;
        public RaycastHit2D right;
        public RaycastHit2D downLeft;
        public RaycastHit2D downRight;
    }
    protected EnemyRaycasts raycasts;

    protected virtual void Awake()
    {
        health = maxHealth;
        rend = GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        normalShader = Shader.Find("Sprites/Default");
        shaderSpritesDefault = rend.material.shader;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        speed = maxSpeed;
    }

    protected virtual void Update()
    {
        UpdateRaycasts();
        if(state == EnemyState.patroling)
        {
            MoveAround();
        }
        transform.position += new Vector3(-transform.localScale.x * speed * Time.deltaTime, rb.velocity.y);
    }

    protected virtual void MoveAround()
    {
        speed = maxSpeed * patrolSpeedMultiplier;
        if(transform.localScale.x < 0f && raycasts.right)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if(transform.localScale.x > 0f && raycasts.left)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    protected void UpdateRaycasts()
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
        Vector3 toTarget = targetPos - transform.position;
        speed = maxSpeed;
        if(toTarget.x > 1f && transform.localScale.x > 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(toTarget.x < -1f && transform.localScale.x < 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if(toTarget.x > -1f && toTarget.x < 1f)
        {
            speed = 0f;
        }
    }

    protected IEnumerator BeExhausted()
    {
        // TODO set animation to exhausted
        for (float t = 0; t < exhaustionTime * 0.33333f; t += Time.deltaTime)
        {
            speed = maxSpeed * (1 - (t / (exhaustionTime * 0.33333f)));
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0; t < exhaustionTime * 0.66666f; t += Time.deltaTime)
        {
            speed = 0f;
            yield return new WaitForEndOfFrame();
        }
        exhaustionCounter = 0f;
        state = EnemyState.patroling;
    }

    public void TakeDamage(int damage, Vector2 knockback, float knockedBackDur, bool isCrit = false)
    {
        StartCoroutine(SetBackToDefaultShader(flashDuration, isCrit));
        health -= damage;
        rb.velocity = new Vector2(knockback.x, 0f) * Time.deltaTime;
        StartCoroutine(GetKnockedBackForSeconds(knockedBackDur, isCrit));
        if(health <= 0)
        {
            Die();
        }
    }

    protected IEnumerator GetKnockedBackForSeconds(float seconds, bool isCrit = false)
    {
        state = EnemyState.knockedBack;
        if(!isCrit)
        {
            yield return new WaitForSeconds(seconds);
        }
        else
        {
            yield return new WaitForSeconds(seconds * 1.5f);
        }
        state = EnemyState.patroling;
    }

    /// <summary>
    /// Make the Sprite show the normal colors again after a set amount of time
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    protected IEnumerator SetBackToDefaultShader(float sec, bool isCrit)
    {
        if(!isCrit)
        {
            // Let the enemy sprite flash up white
            rend.material.shader = shaderGUItext;
            rend.color = flashUpColor;
        }
        else
        {
            // Let the enemy sprite flash up red
            rend.material.shader = shaderGUItext;
            rend.color = Color.red;
        }
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

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerController>().TakeDamage(attack, (collision.transform.position - transform.position).normalized * knockBackStrength, knockBackDuration);
        }
    }
}
