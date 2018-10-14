using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    //public float speed = 6.0F;
    //public float jumpSpeed = 8.0F;
    //public float gravity = 20.0F;
    //public float rotateSpeed = 6;
    //public bool limitDiagonalSpeed = true;
    ////public float DashDistance = 5f;
    ////public Vector3 Drag = new Vector3(1, 1, 1);
    //private Vector3 moveDirection = Vector3.zero;

    //public CharacterController controller;

    //public Transform forwardObject;
    //public GameObject PlayerModel;

    //// Update is called once per frame
    //void Update()
    //{
    //    float inputX = Input.GetAxis("Horizontal");
    //    float inputY = Input.GetAxis("Vertical");
    //    // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
    //    float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

    //    if (controller.isGrounded)
    //    {
    //        moveDirection = new Vector3(inputX, 0, inputY);
    //        moveDirection = transform.TransformDirection(moveDirection);
    //        moveDirection *= speed;

    //        //transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

    //        if (Input.GetButton("Jump"))
    //            moveDirection.y = jumpSpeed;

    //    }
    //    else
    //    {

    //    }
    //    moveDirection.y -= gravity * Time.deltaTime;
    //    print(moveDirection.y);

    //    controller.Move(forwardObject.TransformDirection(moveDirection) * Time.deltaTime);

    //    //characterController.Move(forwardObject.TransformDirection(moveDirection) * Time.deltaTime * moveSpeed); // not physics friendly
    //    Vector3 movement = new Vector3(forwardObject.TransformDirection(moveDirection).x, 0.0f, forwardObject.TransformDirection(moveDirection).z);
    //    if (movement != Vector3.zero)
    //    {
    //        PlayerModel.transform.rotation = Quaternion.Lerp(PlayerModel.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 20);
    //    }
    //}


    //larger character controller code
    //code referenced here http://wiki.unity3d.com/index.php?title=FPSWalkerEnhanced

    public float walkSpeed = 6.0f;

    public float runSpeed = 11.0f;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    public bool limitDiagonalSpeed = true;

    // If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
    // There must be a button set up in the Input Manager called "Run"
    public bool toggleRun = false;

    public float jumpSpeed = 8.0f;
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
    public int antiBunnyHopFactor = 1;

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
    private int jumpTimer;
    private bool CanMove;
    private bool CanTurn;

    public Transform forwardObject;
    public GameObject PlayerModel;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
        CanMove = true;
        CanTurn = true;
    }

    void Update()
    {
        // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
        // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
        if (toggleRun && grounded && Input.GetButtonDown("Run"))
            speed = (speed == walkSpeed ? runSpeed : walkSpeed);

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
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

            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ((sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = forwardObject.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetButton("Jump"))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
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
            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = forwardObject.TransformDirection(moveDirection);
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something

        if (CanMove)
        {
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
        if (CanTurn)
        {
            Vector3 movement = new Vector3((moveDirection).x, 0.0f, (moveDirection).z);
            if (movement != Vector3.zero)
            {
                PlayerModel.transform.rotation = Quaternion.Lerp(PlayerModel.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 20);
            }
        }
    }

    //// Store point that we're in contact with for use in FixedUpdate if needed
    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    contactPoint = hit.point;
    //    print(hit.gameObject);
    //}

    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert(float fallDistance)
    {
        print("Ouch! Fell " + fallDistance + " units!");
    }

    //Physics code, need to add colider and rigid body to player with drag,


    //public float Speed = 5f;
    //public float JumpHeight = 2f;
    //public float GroundDistance = 0.2f;
    //public float DashDistance = 5f;
    //public LayerMask Ground;
    //public Transform GroundTouch;

    //private Rigidbody _body;
    //private Vector3 _inputs = Vector3.zero;
    //private bool _isGrounded = true;

    //void Start()
    //{
    //    _body = GetComponent<Rigidbody>();
    //}

    //void Update()
    //{
    //    _isGrounded = Physics.CheckSphere(GroundTouch.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

    //    _inputs = Vector3.zero;
    //    _inputs.x = Input.GetAxis("Horizontal");
    //    _inputs.z = Input.GetAxis("Vertical");
    //    _inputs = forwardObject.TransformDirection(_inputs);
    //    _inputs.y = 0;
    //    //if (_inputs != Vector3.zero)
    //    //transform.forward = _inputs;

    //    Vector3 movement = new Vector3(_inputs.x, 0.0f, _inputs.z);
    //    if (movement != Vector3.zero)
    //    {
    //        PlayerModel.transform.rotation = Quaternion.Lerp(PlayerModel.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 20);
    //    }

    //    if (Input.GetButtonDown("Jump") && _isGrounded)
    //    {
    //        _body.velocity += jumpSpeed * Vector3.up;
    //    }
    //    if (Input.GetKeyDown(KeyCode.LeftControl))
    //    {
    //        print("dash");
    //        Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
    //        print((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime));
    //        _body.AddForce(dashVelocity, ForceMode.VelocityChange);
    //    }
    //}


    //void FixedUpdate()
    //{
    //    _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
    //}

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

    public IEnumerator KnockbackPlayer(Vector3 direction, float speed, float time)
    {
        float count = 0;
        PreventTuring();
        PreventMoving();
        while (count <= time)
        {
            yield return null;
            count += Time.deltaTime;
            grounded = (controller.Move(direction * speed * Time.deltaTime) & CollisionFlags.Below) != 0;

        }

        AllowTurning();
        AllowMoving();

    }
}
