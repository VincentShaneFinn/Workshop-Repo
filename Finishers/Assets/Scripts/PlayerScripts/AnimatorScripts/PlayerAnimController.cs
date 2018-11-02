using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerActions {idle,slashL,slashR,jump,dodge,finish }
public class PlayerAnimController : MonoBehaviour {
    public PlayerMovementController pmc=null;

    public Rigidbody rig = null;
    public Transform model = null;
    public Animator anim = null;
    public PlayerActions next = PlayerActions.idle;
    public FinisherMode MyFinisherMode;

    public float ActionCooldownTime = .1f;
    private float ActionCooldownCount;
    private bool LastPrimaryAttackWasSlashL = false;
    private float attackTime;
    private float attackCount;

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
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "SlashL":
                    attackTime = clip.length;
                    break;
                default:
                    break;
            }
        }
        attackCount = attackTime;

        ActionCooldownCount = ActionCooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (GameStatus.FinisherModeActive) // separate player input style while in finisher mode
        {
            return;
        }

        if (!GameStatus.GamePaused)
        {

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
            //Temporary for siphoning attack
            if (Input.GetButtonDown("SecondaryAttack"))
            {
                next = PlayerActions.slashR;
            }
            if (Input.GetButtonDown("Jump")) //dodge is handled in player movement controller, jumping with animation cant actually jump on a platform
            {
                if (GameStatus.InCombat)
                    next = PlayerActions.dodge;
                // else if (state.IsName("idle")) // we dont want jumping to be part of the input que
                //anim.Play("Jump");

            }
            if (Input.GetButtonDown("FinishMode"))
            {
                next = PlayerActions.finish;
            }
        }
        if (state.IsName("idle")||state.IsName("Jump"))
        {
            pmc.AllowTurning();
            pmc.AllowMoving();
        }

        //this is the "Que" that gathers the next action and makes it happen the next time the player is idle
        //need to make room for some animation to cancel halfway through an animation
        if (state.IsName("idle"))
        {

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
                    anim.Play("FinisherRunicIdleStance");
                    break;
                case PlayerActions.slashL:
                    anim.Play("SlashL");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    StartCoroutine(pmc.StepForward(attackTime)); //MARK: Find a way to get the actual animation time
                    LastPrimaryAttackWasSlashL = true;
                    attackCount = attackTime;
                    break;
                case PlayerActions.slashR:
                    anim.Play("SlashR");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    StartCoroutine(pmc.StepForward(attackTime));
                    LastPrimaryAttackWasSlashL = false;
                    attackCount = attackTime;
                    break;
            }
            if (next != PlayerActions.idle)
                ActionCooldownCount = 0;
            next = PlayerActions.idle;
        }
        else if (attackCount < attackTime / 2) //put stuff here that should be able to cut attack off halfway
        {
            switch (next)
            {
                case PlayerActions.dodge:
                    anim.Play("idle"); //will be dodge in the future
                    pmc.dashed = true;
                    next = PlayerActions.idle;
                    break;
                case PlayerActions.finish:
                    anim.Play("FinisherRunicIdleStance"); //start finisher animation
                    MyFinisherMode.TryFinisher = true;
                    break;
            }
        }
        attackCount -= Time.deltaTime;

    }
}
