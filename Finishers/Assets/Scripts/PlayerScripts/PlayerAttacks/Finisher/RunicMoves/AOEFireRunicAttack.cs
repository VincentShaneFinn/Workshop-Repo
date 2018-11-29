using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEFireRunicAttack : MonoBehaviour {

    private float damage;

    void Start()
    {
        damage = PlayerDamageValues.Instance.FlameAOEDamage;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundController>().PlayFlameAOE();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            col.gameObject.GetComponent<Enemyhp>().damage(damage, AttackType.Fire);
            Debug.Log(damage);
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            col.gameObject.GetComponent<Enemyhp>().damage(damage, AttackType.Fire);
        }
    }
}
