using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicBeamBlast : MonoBehaviour {

    public GameObject parent;
    public float DestroyTime;
    private float DestroyCount;
    
	// Use this for initialization
	void Start () {
        DestroyCount = DestroyTime;
	}
	
	// Update is called once per frame
	void Update () {
        DestroyCount -= Time.deltaTime;
        if (DestroyCount <= 0)
        {
            Destroy(parent);
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Destroy(col.gameObject);
        }
    }
}
