using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    #region Fields

    public event System.Action OnShotFired;

    [SerializeField] BoxCollider2D colliderDefiningRaycasts;

    struct PlayerRaycasts // To store the informations of raycasts around the player to calculate physics
    {
        public RaycastHit2D bottomLeft;
        public RaycastHit2D bottomRight;
        public RaycastHit2D bottomCenter;
        public RaycastHit2D upperLeft;
        public RaycastHit2D lowerLeft;
        public RaycastHit2D upperRight;
        public RaycastHit2D lowerRight;
        public RaycastHit2D top;
    }
    PlayerRaycasts raycasts; // Stores the actual information of the raycasts to calculate physics


    [SerializeField] float speed = 1f;
    [SerializeField] float speedWhileShooting = 1f;
    [SerializeField] float backwardsSpeed = 1f;
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float jumpHoldUpGain = 1f;
    Vector3 velocity;
    bool isGrounded = false;
    bool isOnWall = false;
    [SerializeField] float veloYLimit = 1f;
    [SerializeField] float gravity = 1f;

    PlayerInput input;
    Animator anim;

    CameraShake camShake;

    [SerializeField] GameObject gunHolder;
    GunManager equippedGun;
    float timeWhenLastShot;

    [SerializeField] float collectItemRange = 1f;

    [SerializeField] float shakeDurWhenShotFired = 0.2f;

    LayerMask layersToCollideWith;

    #endregion

    #region Unity Messages

    void Awake () {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        camShake = Camera.main.GetComponent<CameraShake>();
        int layer = LayerMask.NameToLayer("SolidObjects");
        layersToCollideWith = 1 << layer;
	}
	
	void FixedUpdate ()
    {
        UpdateRaycasts();
        CheckGrounded();
        if(input.Horizontal > 0f && transform.localScale.x > 0 || input.Horizontal < 0f && transform.localScale.x < 0)
        {
            if(!input.Shoot)
            {
                velocity.x = input.Horizontal * speed * Time.fixedDeltaTime;
            }
            else
            {
                velocity.x = input.Horizontal * speedWhileShooting * Time.fixedDeltaTime;
            }
        }
        else
        {
            velocity.x = input.Horizontal * backwardsSpeed * Time.fixedDeltaTime;
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
                InitiateShoot();
            }
        }
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += -gravity * Time.fixedDeltaTime;
        }
        CheckForValidVelocity();
        if (input.Jump == 1 && isGrounded)
        {
            velocity.y += jumpForce * Time.fixedDeltaTime;
        }
        if(input.Jump == 2 && !isGrounded)
        {
            velocity.y += jumpHoldUpGain * Time.fixedDeltaTime;
        }
        transform.position += velocity;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Make sure the velocity does not violate the laws of physics in this game
    /// </summary>
    protected void CheckForValidVelocity()
    {
        // Check for ground under the player
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        // Checking for colliders to the sides
        if (WallInWay())
        {
            velocity.x = 0f;
        }

        // Make sure, velocity in y axis does not get over limit
        //if (velocity.y >= 0 && velocity.y > veloYLimit)
        //{
        //    velocity.y = veloYLimit;
        //}
        if (velocity.y <= 0 && velocity.y < -veloYLimit)
        {
            velocity.y = -veloYLimit * Time.fixedDeltaTime;
        }

        // Check if something is above the player and let him bounce down again relative to the force he went up with
        if (raycasts.top && velocity.y > 0)
        {
            velocity.y = (-velocity.y / 2) * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Checks if there are walls in the direction the player is facing
    /// </summary>
    /// <returns> True if there is a wall. False when there is none</returns>
    protected bool WallInWay()
    {
        if (transform.localScale.x < 0)
        {
            if (raycasts.upperLeft || raycasts.lowerLeft)
            {
                isOnWall = true;
                if (raycasts.upperLeft.distance < 0.2f && raycasts.upperLeft)
                {
                    transform.position += Vector3.right * ((0.25f - (raycasts.upperLeft.distance)) / 5f);
                }
                else if (raycasts.lowerLeft.distance < 0.2f && raycasts.lowerLeft)
                {
                    transform.position += Vector3.right * ((0.25f - (raycasts.lowerLeft.distance)) / 5f);
                }
                return true;
            }
        }
        else if (transform.localScale.x > 0)
        {
            if (raycasts.upperRight || raycasts.lowerRight)
            {
                isOnWall = true;
                if (raycasts.upperRight.distance < 0.2f && raycasts.upperRight)
                {
                    transform.position += Vector3.left * ((0.25f - (raycasts.upperRight.distance)) / 5f);
                }
                else if (raycasts.lowerRight.distance < 0.2f && raycasts.lowerRight)
                {
                    transform.position += Vector3.left * ((0.25f - (raycasts.lowerRight.distance)) / 5f);
                }
                return true;
            }
        }
        isOnWall = false;
        return false;
    }

    /// <summary>
    /// Checks if the player is on the ground or not
    /// </summary>
    protected virtual void CheckGrounded()
    {
        // When the bottom raycasts hit ground
        if (raycasts.bottomLeft || raycasts.bottomRight || raycasts.bottomCenter)
        {
            isGrounded = true;
            if (raycasts.bottomLeft.distance < 0.2f)
            {
                transform.position += Vector3.up * ((0.25f - (raycasts.bottomLeft.distance)) / 2f);
            }
            else if (raycasts.bottomRight.distance < 0.2f)
            {
                transform.position += Vector3.up * ((0.25f - (raycasts.bottomRight.distance)) / 2f);
            }
            else if(raycasts.bottomCenter.distance < 0.2f)
            {
                transform.position += Vector3.up * ((0.25f - (raycasts.bottomRight.distance)) / 2f);
            }
        }
        // Otherwise the player is not grounded
        else
        {
            isGrounded = false;
        }
    }

    void UpdateRaycasts()
    {
        Vector3 extents = colliderDefiningRaycasts.bounds.extents;
        Vector3 center = colliderDefiningRaycasts.bounds.center;
        raycasts.bottomRight = Physics2D.Raycast(center + new Vector3(extents.x, -extents.y * 0.95f, 0f), Vector2.down, 0.25f, layersToCollideWith);
        raycasts.bottomLeft = Physics2D.Raycast(center + new Vector3(-extents.x, -extents.y * 0.95f, 0f), Vector2.down, 0.25f, layersToCollideWith);
        raycasts.bottomCenter = Physics2D.Raycast(center + new Vector3(0f, -extents.y, 0f), Vector2.down, 0.25f, layersToCollideWith);

        raycasts.upperRight = Physics2D.Raycast(center + new Vector3(extents.x, extents.y, 0f), Vector2.right, 0.25f, layersToCollideWith);
        raycasts.lowerRight = Physics2D.Raycast(center + new Vector3(extents.x, -extents.y, 0f), Vector2.right, 0.25f, layersToCollideWith);

        raycasts.upperLeft = Physics2D.Raycast(center + new Vector3(-extents.x, extents.y, 0f), Vector2.left, 0.25f, layersToCollideWith);
        raycasts.lowerLeft = Physics2D.Raycast(center + new Vector3(-extents.x, -extents.y, 0f), Vector2.left, 0.25f, layersToCollideWith);

        raycasts.top = Physics2D.Raycast(center + new Vector3(0f, extents.y, 0f), Vector2.up, 0.25f, layersToCollideWith);
    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay(colliderDefiningRaycasts.bounds.center + new Vector3(colliderDefiningRaycasts.bounds.extents.x, colliderDefiningRaycasts.bounds.extents.y, 0f), Vector2.right * 0.25f);
    //    Debug.DrawRay(colliderDefiningRaycasts.bounds.center + new Vector3(colliderDefiningRaycasts.bounds.extents.x, -colliderDefiningRaycasts.bounds.extents.y, 0f), Vector2.right * 0.25f);
    //}

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

    private void InitiateShoot()
    {
        anim.SetTrigger("ShotFired");
        OnShotFired();
        timeWhenLastShot = Time.realtimeSinceStartup;
        velocity.x += -transform.localScale.x * equippedGun.Recoil;
        camShake.shakeAmount = equippedGun.Recoil;
        camShake.shakeDuration = shakeDurWhenShotFired;
    }

    #endregion
}
