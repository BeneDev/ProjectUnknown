using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour {

    public bool IsShooting
    {
        get
        {
            return input.Shoot;
        }
    }



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
        public RaycastHit2D centerLeft;
        public RaycastHit2D upperRight;
        public RaycastHit2D lowerRight;
        public RaycastHit2D centerRight;
        public RaycastHit2D top;
        public RaycastHit2D topRight;
        public RaycastHit2D topLeft;
    }
    PlayerRaycasts raycasts; // Stores the actual information of the raycasts to calculate physics

    enum PlayerState
    {
        free,
        blocked,
        dodging
    }
    PlayerState state = PlayerState.free;

    [SerializeField] float speed = 1f;
    [SerializeField] float speedWhileShooting = 1f;
    [SerializeField] float backwardsSpeed = 1f;
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float jumpHoldUpGain = 1f;
    bool stillHasToJump = false;
    Vector3 velocity;
    bool isGrounded = false;
    bool isAgainstWall = false;
    [SerializeField] float veloYLimit = 1f;
    [SerializeField] float gravity = 1f;

    [SerializeField] float dodgePower = 5f;
    [SerializeField] float upwardsDodgePower = 5f;
    float appliedUpwardsDodgePower;
    [SerializeField] float dodgeCooldown = 0.3f;

    PlayerInput input;
    Animator anim;

    CameraShake camShake;

    [SerializeField] GameObject gunHolder;
    [SerializeField] float gunDrag = 5f;
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
        WallInWay();
        CheckGrounded();
        if(state == PlayerState.free)
        {
            // Set the speed for moving the character, depending on how the player wants to move
            if(!equippedGun)
            {
                velocity.x = input.Horizontal * speed * Time.fixedDeltaTime;
            }
            else if (input.Horizontal > 0f && transform.localScale.x > 0 || input.Horizontal < 0f && transform.localScale.x < 0)
            {
                if (!input.Shoot)
                {
                    velocity.x = input.Horizontal * speed * Time.fixedDeltaTime;
                }
                else
                {
                    velocity.x = input.Horizontal * speedWhileShooting * Time.fixedDeltaTime;
                }
            }
            else if(equippedGun && input.Shoot)
            {
                velocity.x = input.Horizontal * backwardsSpeed * Time.fixedDeltaTime;
            }
            else
            {
                velocity.x = 0f;
            }
            ChangeDirection();
            // Check for guns on the ground to pick up
            if (input.Interact)
            {
                Collider2D[] objects = Physics2D.OverlapBoxAll(transform.position, Vector2.one * collectItemRange, 0f);
                CheckForGuns(objects);
            }
            // Shoot
            if (input.Shoot && equippedGun)
            {
                if (Time.realtimeSinceStartup >= timeWhenLastShot + equippedGun.ShotDelay)
                {
                    InitiateShoot();
                }
            }
            if(input.Dodge)
            {
                state = PlayerState.dodging;
                anim.SetTrigger("Dodge");
                velocity.y = upwardsDodgePower * Time.fixedDeltaTime;
            }
        }
        if(state == PlayerState.dodging)
        {
            velocity.x = transform.localScale.x * dodgePower * Time.fixedDeltaTime;
        }
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += -gravity * Time.fixedDeltaTime;
        }
        if(state == PlayerState.free)
        {
            // Check for jumps
            if(input.Jump == 1)
            {
                StartCoroutine(KeepJumpForFrames(30));
            }
            if (input.Jump == 1 && isGrounded || stillHasToJump && isGrounded)
            {
                velocity.y += jumpForce * Time.fixedDeltaTime;
                stillHasToJump = false;
            }
            if (input.Jump == 2 && !isGrounded)
            {
                velocity.y += jumpHoldUpGain * Time.fixedDeltaTime;
            }
        }
        CheckForValidVelocity();
        // Apply the velocity
        anim.SetFloat("YVelo", velocity.y);
        transform.position += velocity;
    }

    #endregion

    #region Helper Methods

    IEnumerator KeepJumpForFrames(int framecount)
    {
        stillHasToJump = true;
        for (int i = 0; i < framecount; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        stillHasToJump = false;
    }

    /// <summary>
    /// Make sure the velocity does not violate the laws of physics in this game
    /// </summary>
    protected void CheckForValidVelocity()
    {
        if(CheckGrounded() != null)
        {
            RaycastHit2D hittingRay = (RaycastHit2D)CheckGrounded();
            // Check for ground under the player
            if (velocity.y < 0f && hittingRay.distance < 0.3f && hittingRay.distance > 0.2f)
            {
                velocity.y = 0;
            }
            if (velocity.y < 0)
            {
                velocity.y *= 0.5f;
            }
            if (velocity.y < 0 && hittingRay.distance < 0.2f)
            {
                transform.position += Vector3.up * ((0.25f - (hittingRay.distance)));
            }
        }

        // Checking for colliders to the sides
        if (isAgainstWall)
        {
            velocity.x = 0f;
        }
        
        // Make sure, the y velocity stays in the velocity limit
        if (velocity.y <= 0 && velocity.y < -veloYLimit)
        {
            velocity.y = -veloYLimit * Time.fixedDeltaTime;
        }

        // Check if something is above the player and let him bounce down again relative to the force he went up with
        if(velocity.y > 0)
        {
            if (raycasts.top || raycasts.topRight || raycasts.topLeft)
            {
                velocity.y = (-velocity.y / 2) * Time.fixedDeltaTime;
            }
        }
    }

    public void EndDodge()
    {
        StartCoroutine(SlowDodgeDown(dodgeCooldown));
        state = PlayerState.blocked;
    }

    IEnumerator SlowDodgeDown(float seconds)
    {
        float maxVelo = velocity.x;
        for(float t = 0; t < seconds; t += Time.deltaTime)
        {
            velocity.x = maxVelo * (1 - (t / seconds));
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0; t < seconds * 0.25f; t += Time.deltaTime)
        {
            velocity.x = input.Horizontal * (speed * 0.5f) * Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        state = PlayerState.free;
    }

    /// <summary>
    /// Checks if there are walls in the direction the player is facing
    /// </summary>
    /// <returns> True if there is a wall. False when there is none</returns>
    protected bool WallInWay()
    {
        if (raycasts.upperLeft || raycasts.lowerLeft)
        {
            if(transform.localScale.x < 0)
            {
                isAgainstWall = true;
            }
            if (raycasts.upperLeft.distance < 0.2f && raycasts.upperLeft)
            {
                transform.position += Vector3.right * ((0.25f - (raycasts.upperLeft.distance)) / 5f);
            }
            else if (raycasts.lowerLeft.distance < 0.2f && raycasts.lowerLeft)
            {
                transform.position += Vector3.right * ((0.25f - (raycasts.lowerLeft.distance)) / 5f);
            }
            else if (raycasts.centerLeft.distance < 0.2f && raycasts.lowerLeft)
            {
                transform.position += Vector3.right * ((0.25f - (raycasts.centerLeft.distance)) / 5f);
            }
            return true;
        }
        else if (raycasts.upperRight || raycasts.lowerRight)
        {
            if (transform.localScale.x > 0)
            {
                isAgainstWall = true;
            }
            if (raycasts.upperRight.distance < 0.2f && raycasts.upperRight)
            {
                transform.position += Vector3.left * ((0.25f - (raycasts.upperRight.distance)) / 5f);
            }
            else if (raycasts.lowerRight.distance < 0.2f && raycasts.lowerRight)
            {
                transform.position += Vector3.left * ((0.25f - (raycasts.lowerRight.distance)) / 5f);
            }
            else if (raycasts.centerRight.distance < 0.2f && raycasts.lowerRight)
            {
                transform.position += Vector3.left * ((0.25f - (raycasts.centerRight.distance)) / 5f);
            }
            return true;
        }
        isAgainstWall = false;
        return false;
    }

    /// <summary>
    /// Checks if the player is on the ground or not
    /// </summary>
    protected virtual RaycastHit2D? CheckGrounded()
    {
        // When the bottom raycasts hit ground
        if (raycasts.bottomLeft)
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
            return raycasts.bottomLeft;
        }
        else if(raycasts.bottomRight)
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
            return raycasts.bottomRight;
        }
        else if(raycasts.bottomCenter)
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
            return raycasts.bottomCenter;
        }
        // Otherwise the player is not grounded
        //else if(!raycasts.bottomLeft && !raycasts.bottomRight && !raycasts.bottomCenter)
        //{
        //    isGrounded = false;
        //}
        isGrounded = false;
        anim.SetBool("Grounded", isGrounded);
        return null;
    }

    void UpdateRaycasts()
    {
        Vector3 extents = colliderDefiningRaycasts.bounds.extents;
        Vector3 center = colliderDefiningRaycasts.bounds.center;
        raycasts.bottomRight = Physics2D.Raycast(center + new Vector3(extents.x, -extents.y * 0.95f, 0f), Vector2.down, 0.5f, layersToCollideWith);
        raycasts.bottomLeft = Physics2D.Raycast(center + new Vector3(-extents.x, -extents.y * 0.95f, 0f), Vector2.down, 0.5f, layersToCollideWith);
        raycasts.bottomCenter = Physics2D.Raycast(center + new Vector3(0f, -extents.y, 0f), Vector2.down, 0.5f, layersToCollideWith);

        raycasts.upperRight = Physics2D.Raycast(center + new Vector3(extents.x, extents.y, 0f), Vector2.right, 0.25f, layersToCollideWith);
        raycasts.lowerRight = Physics2D.Raycast(center + new Vector3(extents.x, -extents.y, 0f), Vector2.right, 0.25f, layersToCollideWith);
        raycasts.centerRight = Physics2D.Raycast(center + new Vector3(extents.x, 0f, 0f), Vector2.right, 0.25f, layersToCollideWith);

        raycasts.upperLeft = Physics2D.Raycast(center + new Vector3(-extents.x, extents.y, 0f), Vector2.left, 0.25f, layersToCollideWith);
        raycasts.lowerLeft = Physics2D.Raycast(center + new Vector3(-extents.x, -extents.y, 0f), Vector2.left, 0.25f, layersToCollideWith);
        raycasts.centerLeft = Physics2D.Raycast(center + new Vector3(-extents.x, 0f, 0f), Vector2.left, 0.25f, layersToCollideWith);

        raycasts.top = Physics2D.Raycast(center + new Vector3(0f, extents.y, 0f), Vector2.up, 0.25f, layersToCollideWith);
        raycasts.topRight = Physics2D.Raycast(center + new Vector3(extents.x, extents.y, 0f), Vector2.up, 0.25f, layersToCollideWith);
        raycasts.topLeft = Physics2D.Raycast(center + new Vector3(-extents.x, extents.y, 0f), Vector2.up, 0.25f, layersToCollideWith);
    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay(colliderDefiningRaycasts.bounds.center + new Vector3(colliderDefiningRaycasts.bounds.extents.x, -colliderDefiningRaycasts.bounds.extents.y, 0f), Vector2.down * 0.25f);
    //    Debug.DrawRay(colliderDefiningRaycasts.bounds.center + new Vector3(-colliderDefiningRaycasts.bounds.extents.x, -colliderDefiningRaycasts.bounds.extents.y, 0f), Vector2.down * 0.25f);
    //    Debug.DrawRay(colliderDefiningRaycasts.bounds.center + new Vector3(0f, -colliderDefiningRaycasts.bounds.extents.y, 0f), Vector2.down * 0.25f);
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
                gun.gameObject.GetComponent<GunManager>().Equip(gameObject);
                gun.gameObject.transform.parent = gunHolder.transform;
                gun.transform.localPosition = Vector3.zero;
                equippedGun = gun.gameObject.GetComponent<GunManager>();
                break;
            }
        }
    }

    private void ChangeDirection()
    {
        if(input.Shoot && equippedGun) { return; }
        if (input.Horizontal < 0f && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            transform.position += new Vector3(-1f, 0f, 0f);
        }
        else if (input.Horizontal > 0f && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.position += new Vector3(1f, 0f, 0f);
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
