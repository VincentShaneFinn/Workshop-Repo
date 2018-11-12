using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFireCircle : FinisherAbstract {
    // Use this for initialization
    public GameObject FlameCircle;
	void Start () {
        GetComponent<FinisherMode>().AddFinisherMove(this);
	}
	
    public override void startfinisher(FinisherMode f) {
        Transform PlayerGround = GetComponent<PlayerMovementController>().GroundChecker;
        Instantiate(FlameCircle, PlayerGround.position, PlayerGround.rotation);
        f.anim.Play("idle");
        print("Commit Runit Finisher");
    }
}
