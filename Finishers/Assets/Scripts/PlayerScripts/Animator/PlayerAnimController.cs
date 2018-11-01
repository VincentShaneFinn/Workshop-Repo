using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerActions {idle,slashL,slashR,jump }
public class PlayerAnimController : MonoBehaviour {
    public PlayerMovementController pmc=null;

    public Rigidbody rig = null;
    public Transform model = null;
    public Animator anim = null;
    public PlayerActions next = PlayerActions.idle;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStatus.GamePaused)
        {

            if (Input.GetButtonDown("PrimaryAttack"))
            {
                next = PlayerActions.slashL;

            }
            //Temporary for siphoning attack
            if (Input.GetButtonDown("SecondaryAttack"))
            {
                next = PlayerActions.slashR;
            }
            if (!GameStatus.InCombat)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    next = PlayerActions.jump;
                }
            }
        }
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("idle")||state.IsName("Jump"))
        {
            pmc.AllowTurning();
            pmc.AllowMoving();
        }
        if (state.IsName("idle"))
        {
            switch (next)
            {
                case PlayerActions.jump:
                    anim.Play("Jump");
                    break;
                case PlayerActions.slashL:
                    anim.Play("SlashL");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    break;
                case PlayerActions.slashR:
                    anim.Play("SlashR");
                    pmc.PreventMoving();
                    pmc.PreventTuring();
                    break;
            }
            next = PlayerActions.idle;
        }

    }
}
