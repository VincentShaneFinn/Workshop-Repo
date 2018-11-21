using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFrostCircle : FinisherAbstract {
    // Use this for initialization
    public GameObject FrostCircle;

	void Start () {
        GetComponent<FinisherMode>().AddFinisherMove(this);
	}
	
    public override void startfinisher(FinisherMode f) {
        Transform PlayerGround = GetComponent<PlayerMovementController>().GroundChecker;
        Instantiate(FrostCircle, PlayerGround.position, PlayerGround.rotation);
        f.CharAnim.Play("Idle");
        print("Commit Runit Finisher");
    }
}
