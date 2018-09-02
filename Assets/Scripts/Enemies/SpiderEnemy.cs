using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : BaseEnemy {

    protected override void Update()
    {
        UpdateRaycasts();
        toPlayer = player.gameObject.transform.position - transform.position;
        if (state == EnemyState.patroling)
        {
            MoveAround();
            if(toPlayer.magnitude < sightReach)
            {
                state = EnemyState.foundPlayer;
            }
        }
        if(state == EnemyState.foundPlayer)
        {
            MoveTowards(player.gameObject.transform.position);
            if(toPlayer.magnitude > sightReach)
            {
                state = EnemyState.searchPlayer;
            }
        }
        if(state == EnemyState.searchPlayer)
        {
            //TODO make this state make the enemy look around to maybe search the player
            state = EnemyState.patroling;
        }
        transform.position += new Vector3(-transform.localScale.x * speed * Time.deltaTime, rb.velocity.y);
    }

}
