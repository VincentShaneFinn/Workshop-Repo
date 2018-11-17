using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerActions {idle,slashL,slashR,jump,dodge,finish }
public class PlayerAnimController : MonoBehaviour {
    public PlayerMovementController pmc=null;

    public Rigidbody rig = null;
    public Transform model = null;
    public Animator anim = null;
    public Animator CharAnim = null;
    public PlayerActions next = PlayerActions.idle;
    public FinisherMode MyFinisherMode;

    public float ActionCooldownTime = .1f;
    private float ActionCooldownCount;
    private bool LastPrimaryAttackWasSlashL = false;
    private float attack1Time;
    private float attack1Count;
    private float attack2Time;
    private float attack2Count;

    // Use this for initialization
    void Start () {
        if (rig == null) {
            rig = GetComponent<Rigidbody>();
        }
        if (model == null) {
            model = GetComponentInChildren<Transform>();
        }
        if (anim == null) {
            anim = GetComponent<Animator>();
        }
        if (pmc == null)
        {
            pmc = GetComponent<PlayerMovementController>();
        }

        //Get animation clip lengths that we need
        AnimationClip[] clips = CharAnim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Attack 1":
                    attack1Time = clip.length / .75f; // clips are playing at .75 speed
                    break;
                case "Attack 2":
                    attack2Time = clip.length / .75f;// clips are playing at .75 speed
                    break;
                default:
                    break;
            }
        }
        attack1Count = attack1Time;
        attack2Count = attack2Time;

        ActionCooldownCount = ActionCooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo CharState = CharAnim.GetCurrentAnimatorStateInfo(0);

        if (GameStatus.GamePaused)
        {
            anim.updateMode = AnimatorUpdateMode.Normal;
            CharAnim.updateMode = AnimatorUpdateMode.Normal;
            return;
        }
        else
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            CharAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (GameStatus.FinisherModeActive) // separate player input style while in finisher mode
        {
            return;
        }

        //Make sure player cant attack while dodgeing
        if (Input.GetButtonDown("PrimaryAttack"))
        {
            if (!pmc.isDashing())
            {
                //Alternate Primary Attack directions
                if (!LastPrimaryAttackWasSlashL)
                    next = PlayerActions.slashL;
                else
                    next = PlayerActions.slashR;
            }

        }
        //When we get heavy attack, more work to be done
        //if (Input.GetButtonDown("SecondaryAttack"))
        //{
        //    next = PlayerActions.slashR;
        //}
        if (Input.GetButtonDown("Dodge")) //dodge is handled in player movement controller, jumping with animation cant actually jump on a platform
        {
            next = PlayerActions.dodge;
        }
        if (Input.GetButtonDown("FinishMode"))
        {
            next = PlayerActions.finish;
        }
        

        //this is the "Que" that gathers the next action and makes it happen the next time the player is idle
        //need to make room for some animation to cancel halfway through an animation
        if (CharState.IsName("Idle") || CharState.IsName("Run"))
        {
            pmc.AllowTurning();
            pmc.AllowMoving();

            if (ActionCooldownCount < ActionCooldownTime)
            {
                ActionCooldownCount += Time.deltaTime;
                return;
            }

            switch (next)
            {
                case PlayerActions.dodge:
                    pmc.dashed = true; // if not in combat que up the dodge, this needs to be able to cut off an animation halfway
                    break;
                case PlayerActions.finish:
                    MyFinisherMode.TryFinisher = true;
                    break;
                case PlayerActions.slashL:
                    //anim.Play("SlashL");
                    CharAnim.Play("Attack 1");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    StartCoroutine(pmc.StepForward(attack1Time)); //MARK: Find a way to get the actual animation time
                    LastPrimaryAttackWasSlashL = true;
                    attack1Count = attack1Time;
                    break;
                case PlayerActions.slashR:
                    //anim.Play("SlashR");
                    CharAnim.Play("Attack 2");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    StartCoroutine(pmc.StepForward(attack2Time));
                    LastPrimaryAttackWasSlashL = false;
                    attack2Count = attack2Time;
                    break;
            }
            if (next != PlayerActions.idle)
                ActionCooldownCount = 0;
            next = PlayerActions.idle;
        }
        else if ((CharState.IsName("Attack 1") && attack1Count < attack1Time / 2) || (CharState.IsName("Attack 2") && attack2Count < attack2Time / 2)) //put stuff here that should be able to cut attack off halfway
        {
            switch (next)
            {
                case PlayerActions.dodge:
                    if (pmc.dashTimer >= pmc.dashFactor + pmc.dashCooldown)
                    {
                        CharAnim.Play("Idle"); //will be dodge in the future
                        pmc.dashed = true;
                    }
                    next = PlayerActions.idle;
                    break;
                case PlayerActions.finish:
                    MyFinisherMode.TryFinisher = true;
                    break;
            }
        }
        attack1Count -= Time.deltaTime;
        attack2Count -= Time.deltaTime;

    }
}
