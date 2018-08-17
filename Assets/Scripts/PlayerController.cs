using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    #region Fields

    public event System.Action OnShotFired;

    [SerializeField] float speed = 1f;
    [SerializeField] float backwardsSpeed = 1f;
    Vector3 velocity;

    PlayerInput input;
    Animator anim;

    [SerializeField] GameObject gunHolder;
    GunManager equippedGun;
    float timeWhenLastShot;

    [SerializeField] float collectItemRange = 1f;

    #endregion

    #region Unity Messages

    void Awake () {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
	}
	
	void Update ()
    {
        if(input.Horizontal > 0f && transform.localScale.x > 0 || input.Horizontal < 0f && transform.localScale.x < 0)
        {
            velocity.x = input.Horizontal * speed * Time.deltaTime;
        }
        else
        {
            velocity.x = input.Horizontal * backwardsSpeed * Time.deltaTime;
        }
        ChangeDirection();
        if(input.Interact)
        {
            Collider2D[] objects = Physics2D.OverlapBoxAll(transform.position, Vector2.one * collectItemRange, 0f);
            CheckForGuns(objects);
        }
        if (input.Shoot && equippedGun)
        {
            if(Time.realtimeSinceStartup >= timeWhenLastShot + equippedGun.ShotDelay)
            {
                anim.SetTrigger("ShotFired");
                OnShotFired();
                timeWhenLastShot = Time.realtimeSinceStartup;
                velocity.x += -transform.localScale.x * equippedGun.Recoil;
            }
        }
        transform.position += velocity;
    }

    #endregion

    #region Helper Methods

    private void CheckForGuns(Collider2D[] objects)
    {
        foreach (Collider2D gun in objects)
        {
            // If there is a gun
            if (gun.gameObject.GetComponent<GunManager>())
            {
                // Collect the gun
                if (equippedGun)
                {
                    equippedGun.Unequip();
                }
                gun.gameObject.GetComponent<GunManager>().Equip(GetComponent<PlayerController>());
                gun.gameObject.transform.parent = gunHolder.transform;
                gun.transform.localPosition = Vector3.zero;
                equippedGun = gun.gameObject.GetComponent<GunManager>();
                break;
            }
        }
    }

    private void ChangeDirection()
    {
        if(input.Shoot) { return; }
        if (input.Horizontal < 0f && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (input.Horizontal > 0f && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    #endregion
}
