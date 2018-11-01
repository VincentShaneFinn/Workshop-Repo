using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationwrapper : MonoBehaviour {
    public Transform model;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation.Set (model.localRotation.x, model.localRotation.y, model.localRotation.z, model.localRotation.w);
        model.localRotation = Quaternion.identity;
	}
}
