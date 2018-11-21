using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostConeDamge : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        Enemyhp ehp = col.gameObject.GetComponent<Enemyhp>();
        if (col.gameObject.tag == "Enemy")
        {
            ehp.damage(PlayerDamageValues.Instance.FrostAOEExplodeDamge);
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameThrowFinMeterFill);
            col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameThrowFinMeterFill);
            ehp.damage(PlayerDamageValues.Instance.FrostAOEExplodeDamge);
        }
    }
}
