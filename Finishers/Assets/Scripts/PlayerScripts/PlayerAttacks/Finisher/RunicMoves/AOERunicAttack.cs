using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOERunicAttack : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter();
            col.gameObject.GetComponent<Enemyhp>().damage(3);
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter();
            col.gameObject.GetComponent<Enemyhp>().damage(3);
        }
    }
}
