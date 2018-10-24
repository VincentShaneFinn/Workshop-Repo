using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyattack : MonoBehaviour {
    public GameObject p;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*
           enemy AI call atk()
         */
	}
    public void atk() {
        GameObject t = Instantiate(p, transform.position, transform.rotation);
        t.transform.parent = gameObject.transform;
        
    }
}
