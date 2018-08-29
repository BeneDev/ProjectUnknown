using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    PlayerController player;
    Vector3 targetPosition;
    Vector3 offset;

    [SerializeField] float camDelay = 1f;
    Vector3 velocity;
    [SerializeField] float xOffset = 2f;

    [SerializeField] float leaveFightModeTimer = 1f;
    bool isFightMode = false;
    float timeWhenLastShot;
    
	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        offset = transform.position - player.transform.position;
        player.OnLetGoOfFire += SetTimeWhenPlayerStoppedShooting;
	}
	
	void Update () {
        if(player.IsShooting && !isFightMode)
        {
            isFightMode = true;
        }
        else if(!player.IsShooting && isFightMode && Time.realtimeSinceStartup >= timeWhenLastShot + leaveFightModeTimer)
        {
            isFightMode = false;
        }
        if(!isFightMode)
        {
            offset = new Vector3(player.transform.localScale.x * xOffset, offset.y, offset.z);
        }
        else
        {
            offset = new Vector3(player.transform.localScale.x * xOffset * 3f, offset.y, offset.z);
        }
        targetPosition = player.transform.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, camDelay);
	}

    void SetTimeWhenPlayerStoppedShooting(float time)
    {
        timeWhenLastShot = time;
    }
}
