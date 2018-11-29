using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherMeterPickup : MonoBehaviour {

    public float FinisherMeterFill = 20;

    private FinisherMode fm;

    void Start()
    {
        fm = GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>();
    }

    void OnTriggerEnter(Collider col)
    {
        fm.IncreaseFinisherMeter(FinisherMeterFill);
        Destroy(gameObject);
    }

}
