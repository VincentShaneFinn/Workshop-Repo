using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFireCircle : FinisherAbstract {
    // Use this for initialization
    public GameObject FlameCircle;
    // Use this for initialization
    void OnEnable()
    {
        GetComponent<FinisherMode>().AddFinisherMove(this);
    }

    void OnDisable()
    {
        GetComponent<FinisherMode>().RemoveFinisherMove(this);
    }

    public override void startfinisher(FinisherMode f) {
        Transform PlayerGround = GetComponent<PlayerMovementController>().GroundChecker;
        Instantiate(FlameCircle, PlayerGround.position, PlayerGround.rotation);
        f.CharAnim.Play("Idle");
        print("Commit Runit Finisher");
    }
}
