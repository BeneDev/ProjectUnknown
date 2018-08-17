using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    [SerializeField] float speed = 1f;
    Vector3 velocity;

    PlayerInput input;

	void Awake () {
        input = GetComponent<PlayerInput>();
	}
	
	void Update () {
        velocity.x = input.Horizontal * speed * Time.deltaTime;
        transform.position += velocity;
	}
}
