using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFire : FinisherAbstract {
    // Use this for initialization
    public GameObject beamobject;
	void Start () {
        GetComponent<FinisherMode>().AddFinisherMove(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void startfinisher(FinisherMode f) {
        Vector3 rot = f.EnemyFinisherPlacement.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        GameObject beam = Instantiate(beamobject, f.EnemyFinisherPlacement.position, Quaternion.Euler(rot));
        beam.transform.parent = f.PlayerRotWrapper;
        f.anim.Play("idle");
        print("Commit Runit Finisher");
    }
}
