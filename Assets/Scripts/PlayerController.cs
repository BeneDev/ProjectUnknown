using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    public event System.Action OnShotFired;

    [SerializeField] float speed = 1f;
    Vector3 velocity;

    PlayerInput input;
    Animator anim;

    [SerializeField] GameObject gunHolder;
    GunManager equippedGun;
    float timeWhenLastShot;

    [SerializeField] float collectItemRange = 1f;

	void Awake () {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
	}
	
	void Update ()
    {
        velocity.x = input.Horizontal * speed * Time.deltaTime;
        ChangeDirection();
        if(input.Interact)
        {
            Collider2D[] objects = Physics2D.OverlapBoxAll(transform.position, Vector2.one * collectItemRange, 0f);
            foreach (Collider2D gun in objects)
            {
                // If there is a gun
                if (gun.gameObject.GetComponent<GunManager>())
                {
                    // Collect the gun
                    if(equippedGun)
                    {
                        equippedGun.Unequip(GetComponent<PlayerController>());
                    }
                    gun.gameObject.GetComponent<GunManager>().Equip(GetComponent<PlayerController>());
                    gun.gameObject.transform.parent = gunHolder.transform;
                    gun.transform.localPosition = Vector3.zero;
                    equippedGun = gun.gameObject.GetComponent<GunManager>();
                    break;
                }
            }
        }
        if(input.Shoot && equippedGun)
        {
            if(Time.realtimeSinceStartup >= timeWhenLastShot + equippedGun.ShotDelay)
            {
                anim.SetTrigger("ShotFired");
                OnShotFired();
                timeWhenLastShot = Time.realtimeSinceStartup;
            }
        }
        transform.position += velocity;
    }

    private void ChangeDirection()
    {
        if (input.Horizontal < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (input.Horizontal > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
