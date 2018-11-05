using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {

    public GameObject Target;

	// Update is called once per frame
	void Update () {
        Vector3 targetPostition = new Vector3(Target.transform.position.x,
                                this.transform.position.y,
                                Target.transform.position.z);
        this.transform.LookAt(targetPostition);
    }
}
