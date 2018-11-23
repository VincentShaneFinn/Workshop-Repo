﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupController : MonoBehaviour {

    public float HealthMeterFill = 10;

    private PlayerHealthController phc;

    void Start()
    {
        phc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthController>();
    }

    void OnTriggerEnter(Collider col)
    {
        phc.PlayerHealed(HealthMeterFill);
        Destroy(gameObject);
    }
}
