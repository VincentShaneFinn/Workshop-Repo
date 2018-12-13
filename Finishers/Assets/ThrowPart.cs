using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPart : MonoBehaviour {

    public Vector3 force;
    public float torqueValue;
    private bool first = true;

    void FixedUpdate()
    {
        if (first)
        {
            Vector3 Rforces = new Vector3(Random.Range(-force.x, force.x), force.y, Random.Range(-force.z, force.z));
            GetComponent<Rigidbody>().AddForce(Rforces, ForceMode.Acceleration);
            GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1)) * torqueValue);
            first = false;
        }
    }
}
