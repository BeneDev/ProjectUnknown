using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : BaseEnemy {

    protected override void Update()
    {
        UpdateRaycasts();
        toPlayer = player.gameObject.transform.position - transform.position;
        if(speed >= maxSpeed)
        {
            exhaustionCounter += Time.deltaTime;
            if(exhaustionCounter >= maxSpeedStamina)
            {
                state = EnemyState.exhausted;
                StartCoroutine(BeExhausted());
            }
        }
        else
        {
            exhaustionCounter -= Time.deltaTime / 2f;
        }
        if (state == EnemyState.patroling)
        {
            MoveAround();
            if(toPlayer.magnitude < sightReach)
            {
                state = EnemyState.foundPlayer;
            }
        }
        else if(state == EnemyState.foundPlayer)
        {
            MoveTowards(player.gameObject.transform.position);
            if(toPlayer.magnitude > sightReach)
            {
                state = EnemyState.searchPlayer;
            }
        }
        else if(state == EnemyState.searchPlayer)
        {
            //TODO make this state make the enemy look around to maybe search the player
            state = EnemyState.patroling;
        }
        transform.position += new Vector3(-transform.localScale.x * speed * Time.deltaTime, rb.velocity.y);
    }

}
