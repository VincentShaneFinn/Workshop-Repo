using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOERunicAttack : MonoBehaviour {

    private float damage;

    void Start()
    {
        damage = PlayerDamageValues.Instance.FlameAOEDamage;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            col.gameObject.GetComponent<Enemyhp>().damage(damage);
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            col.gameObject.GetComponent<Enemyhp>().damage(damage);
        }
    }
}
