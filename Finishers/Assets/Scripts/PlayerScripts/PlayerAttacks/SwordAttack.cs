﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour {

    public GameObject Sword;
    public float swordCooldown = .3f;
    private float swordCount;

    public float swordAttackTime = .2f;
    private float swordAttackCount;

	// Use this for initialization
	void Start () {
        Sword.SetActive(false);
        swordCount = swordCooldown;
    }
	
	// Update is called once per frame
	void Update () {
        if (swordCount >= swordCooldown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Sword.SetActive(true);
                swordCount = 0;
                swordAttackCount = 0;
                transform.localRotation = Quaternion.Euler(0, 60, 0);
            }
        }
        else
        {
            if (swordAttackCount <= swordAttackTime)
            {
                swordAttackCount += Time.deltaTime;
                transform.Rotate(transform.rotation.x, transform.rotation.y - 120 / swordAttackTime * Time.deltaTime, transform.rotation.z); // broken with small time scale
            }
            else
            {
                Sword.SetActive(false);
            }
            swordCount += Time.deltaTime;
        }
	}
}
