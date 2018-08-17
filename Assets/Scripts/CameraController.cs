using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
    Vector3 targetPosition;
    Vector3 offset;

    [SerializeField] float camDelay = 1f;
    Vector3 velocity;
    [SerializeField] float xOffset = 2f;
    
	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
	}
	
	void Update () {
        offset = new Vector3(player.transform.localScale.x * xOffset, offset.y, offset.z);
        targetPosition = player.transform.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, camDelay);
	}
}
