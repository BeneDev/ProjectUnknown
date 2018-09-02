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
        else if(exhaustionCounter > 0f)
        {
            exhaustionCounter -= Time.deltaTime / 2f;
        }
        if (state == EnemyState.patroling)
        {
            MoveAround();
            RaycastHit2D objectFound = Physics2D.Raycast(transform.position, transform.position + toPlayer, sightReach, playerAndObjectsLayer);
            if(toPlayer.magnitude < sightReach && objectFound)
            {
                if(objectFound.collider.gameObject.tag == "Player")
                {
                    state = EnemyState.foundPlayer;
                }
            }
        }
        else if(state == EnemyState.foundPlayer)
        {
            MoveTowards(player.gameObject.transform.position);
            if(toPlayer.magnitude > sightReach)
            {
                state = EnemyState.searchPlayer;
                timeWhenLastSawPlayer = Time.realtimeSinceStartup;
                lastPosPlayerSeen = player.transform.position;
            }
        }
        else if(state == EnemyState.searchPlayer)
        {
            //TODO make this state make the enemy look around to maybe search the player
            MoveTowards(lastPosPlayerSeen);
            // if enemy is where player was last seen, make it look to the other direction, wait 1 sec, and turn again, wait 0.5 seconds and then go into patroling again
            if(Time.realtimeSinceStartup >= timeWhenLastSawPlayer + timeToSearchForPlayer)
            {
                state = EnemyState.patroling;
            }
        }
        transform.position += new Vector3(-transform.localScale.x * speed * Time.deltaTime, rb.velocity.y);
    }

}
