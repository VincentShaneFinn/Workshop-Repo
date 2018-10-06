using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPart : MonoBehaviour {

    public Vector3 force;
    public float torqueValue;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(force);
        GetComponent<Rigidbody>().AddTorque(transform.right * torqueValue);
    }
}
