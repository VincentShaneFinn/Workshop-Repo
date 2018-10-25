﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLimb : MonoBehaviour {

	// Use this for initialization
	void Start () {
        firedPressed = false;
	}

    //VERY QUICK THROW TOGETHER STUFF
    private bool firedPressed;
	
	// Update is called once per frame
	void Update () {
        if (firedPressed)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 20);
        }
        if (Input.GetButtonDown("SpecialAttack"))
        {
            firedPressed = true;
            transform.parent = null;
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyAI>().KillEnemy();
            Destroy(gameObject);
        }
    }
}
