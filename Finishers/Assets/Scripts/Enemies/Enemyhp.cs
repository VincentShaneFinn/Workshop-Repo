﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyhp : MonoBehaviour {
    private int currenthp;
    public int hp =1;
    public GameObject BloodTrail;

	// Use this for initialization
	void Start () {
        currenthp = hp;
	}

    void checkhp() {
        print("ow");
        if (currenthp<=0) {
            if (gameObject.tag != "TargetDummy")
                GetComponent<EnemyAI>().KillEnemy();
            else
                Destroy(gameObject);
        }
    }
    public void damage(bool finisher) {
        currenthp--;
        if (finisher) // must be kileld by a finisher
        {
            checkhp();
        }
        var randomRotation = Quaternion.Euler(transform.rotation.x, Random.Range(0, 360), transform.rotation.z);
        GameObject blood = Instantiate(BloodTrail, transform.position, randomRotation);
        Destroy(blood, 1);
    }
    public void damage(int d, bool finisher) {
        currenthp -= d;
        if (finisher)
        {
            checkhp();
        }
        var randomRotation = Quaternion.Euler(transform.rotation.x, Random.Range(0, 360), transform.rotation.z);
        GameObject blood = Instantiate(BloodTrail, transform.position, randomRotation);
        Destroy(blood, 1);
    }
}
