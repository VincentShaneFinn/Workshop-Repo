﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControllerRigidbody : MonoBehaviour
{
     //larger character controller code
    //code referenced here http://wiki.unity3d.com/index.php?title=FPSWalkerEnhanced

    public float walkSpeed = 6.0f;
    public float runSpeed = 11.0f;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    public bool limitDiagonalSpeed = true;

    // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
    // There must be a button set up in the Input Manager called "Run"
    public bool toggleRun = false;

    public float jumpHeight= 2.0f;
    public float gravity = 20.0f;

    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    public float fallingDamageThreshold = 10.0f;

    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;

    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;

    public float slideSpeed = 12.0f;

    // If checked, then the player can change direction while in the air
    public bool airControl = false;

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;

    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public float antiBunnyHopFactor = 1;
    public float dashFactor = 1;
    public float dashCooldown = 1;
    public float dashSpeed = 10;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private bool dashing = false;
    private float jumpTimer;
    private float dashTimer;
    public bool CanMove;
    public bool CanTurn;

    public Transform forwardObject;
    public GameObject PlayerModel;

    private Vector3 dashDirection;

    public Rigidbody myRigidbody;
    public LayerMask Ground;
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
        dashTimer = dashFactor + dashCooldown;
        CanMove = true;
        CanTurn = true;
    }

    void Update()
    {
        grounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
        // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputXRaw = Input.GetAxisRaw("Horizontal");
        float inputYRaw = Input.GetAxisRaw("Vertical");
        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;
        if (grounded)
        {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling)
            {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
            }

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (!toggleRun)
                speed = Input.GetButton("Run") ? runSpeed : walkSpeed;

            //// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            //if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
            //{
            //    Vector3 hitNormal = hit.normal;
            //    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            //    Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
            //    moveDirection *= slideSpeed;
            //    playerControl = false;
            //}
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            //else
            //{
                moveDirection = new Vector3(inputX * inputModifyFactor, 0, inputY * inputModifyFactor);
                moveDirection = forwardObject.TransformDirection(moveDirection) * speed;
                playerControl = true;
            //}

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            // temp switch for dash testing
            if (!GameStatus.InCombat) {
                if (!Input.GetButtonDown("Jump"))
                {
                    jumpTimer += Time.deltaTime;
                }
                else if (jumpTimer >= antiBunnyHopFactor)
                {
                    myRigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                    jumpTimer = 0;
                }
            }

            if (GameStatus.InCombat)
            {
                if (!Input.GetButtonDown("Jump"))
                {
                    dashTimer += Time.deltaTime;
                    if (dashTimer >= dashFactor)
                    {
                        dashing = false;
                    }
                    else {
                        //0 to dash factor and start at dashSpeed and reduce to normal speed
                        float currentDashSpeed = dashSpeed - walkSpeed;
                        moveDirection = dashDirection * (walkSpeed + currentDashSpeed * (1 - dashTimer/dashFactor));
                    }
                }
                else if (dashTimer >= dashFactor + dashCooldown)
                {
                    //do stuff to dodge
                    dashTimer = 0;
                    dashing = true;
                    if (moveDirection.x != 0 || moveDirection.z != 0)
                        dashDirection = forwardObject.TransformDirection(new Vector3(inputXRaw * inputModifyFactor, 0, inputYRaw * inputModifyFactor));
                    else
                        dashDirection = forwardObject.TransformDirection(new Vector3(0, 0, -1));

  
                    //_body.AddForce(dashVelocity, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl && !dashing)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = forwardObject.TransformDirection(moveDirection);
            }
        }

        // Apply gravity

        // Move the controller, and set grounded true or false depending on whether we're standing on something

        if (CanTurn)
        {
            Vector3 movement = new Vector3((moveDirection).x, 0.0f, (moveDirection).z);
            if (movement != Vector3.zero)
            {
                //print((float)(Time.deltaTime + (1.0 - Time.timeScale)));
                //this doesnt get run if the timeScale is 0
                PlayerModel.transform.rotation = Quaternion.Lerp(PlayerModel.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 20);
            }
        }
    }

    private Vector3 desiredVelocity;
    public float slopeRayHeight = 0;
    public float steepSlopeAngle = 45;
    public float slopeThreshold = .2f;
    public CapsuleCollider myCollider;
    void FixedUpdate()
    {
        if (CanMove)
        {
            desiredVelocity = new Vector3(moveDirection.x, myRigidbody.velocity.y, moveDirection.z);
            myRigidbody.velocity = desiredVelocity;
        }
    }

    bool checkMoveableTerrain(Vector3 position, Vector3 desiredDirection, float distance)
    {
        Ray myRay = new Ray(position, desiredDirection); // cast a Ray from the position of our gameObject into our desired direction. Add the slopeRayHeight to the Y parameter.

        RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, distance))
        {
            if (hit.collider.gameObject.tag == "Ground") // Our Ray has hit the ground
            {
                float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal); // Here we get the angle between the Up Vector and the normal of the wall we are checking against: 90 for straight up walls, 0 for flat ground.

                float radius = Mathf.Abs(slopeRayHeight / Mathf.Sin(slopeAngle)); // slopeRayHeight is the Y offset from the ground you wish to cast your ray from.

                if (slopeAngle >= steepSlopeAngle * Mathf.Deg2Rad) //You can set "steepSlopeAngle" to any angle you wish.
                {
                    if (hit.distance - myCollider.radius > Mathf.Abs(Mathf.Cos(slopeAngle) * radius) + slopeThreshold) // Magical Cosine. This is how we find out how near we are to the slope / if we are standing on the slope. as we are casting from the center of the collider we have to remove the collider radius.
                                                                                                                     // The slopeThreshold helps kills some bugs. ( e.g. cosine being 0 at 90° walls) 0.01 was a good number for me here
                    {
                        return true; // return true if we are still far away from the slope
                    }

                    return false; // return false if we are very near / on the slope && the slope is steep
                }

                return true; // return true if the slope is not steep

            }

        }
        return true;
    }


    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }

    public void PreventMoving()
    {
        CanMove = false;
    }
    public void AllowMoving()
    {
        CanMove = true;
    }

    public void PreventTuring()
    {
        CanTurn = false;
    }
    public void AllowTurning()
    {
        CanTurn = true;
    }

    private float KnockbackTimer;
    private float KnockbackCount;
    public PlayerUpdater pUpdater;

    public IEnumerator KnockbackPlayer(GameObject other)
    {
        if (pUpdater.PoiseCount < pUpdater.PoiseTime)
            yield break;
        else
            pUpdater.PoiseCount = 0;
        float time = .15f;
        float speed = 20; // keep greater than 6
        //other.gameObject.GetComponent<EnemyMovementController>().PauseMovement();
        Vector3 dir = (transform.position - other.transform.position).normalized;

        float count = 0;
        PreventTuring();
        PreventMoving();
        while (count <= time)
        {
            yield return null;
            count += Time.deltaTime;
            float currentKnockbackSpeed = speed - walkSpeed;
            myRigidbody.MovePosition(myRigidbody.position + dir * (walkSpeed + currentKnockbackSpeed * (1 - count / time)) * Time.deltaTime);
        }

        AllowTurning();
        AllowMoving();

    }
}
