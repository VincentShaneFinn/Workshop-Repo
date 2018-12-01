using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFlamethrower : FinisherAbstract {
    // Use this for initialization
    public GameObject FlameObject;
    // Use this for initialization
    void OnEnable()
    {
        GetComponent<FinisherMode>().AddFinisherMove(this);
    }

    void OnDisable()
    {
        GetComponent<FinisherMode>().RemoveFinisherMove(this);
    }

    // Update is called once per frame
    void Update () {
		
	}
    public override void startfinisher(FinisherMode f) {
        Vector3 rot = f.EnemyFinisherPlacement.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        GameObject FlameThrower = Instantiate(FlameObject, f.EnemyFinisherPlacement.position, Quaternion.Euler(rot));
        FlameThrower.transform.parent = f.EnemyFinisherPlacement;
        f.CharAnim.Play("Flamethrower");
        print("Commit Runit Finisher");
    }
}
