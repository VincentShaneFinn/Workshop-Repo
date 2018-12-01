using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEFrostRunicAttack : MonoBehaviour {

    private float damage;
    private int freezeCount = 0;
    private int freezeLimit;

    void Start()
    {
        damage = PlayerDamageValues.Instance.FrostAOEDamage;
        freezeLimit = PlayerDamageValues.Instance.FrostAOEFreezeCount;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundController>().PlayFlameAOE();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (col.gameObject.GetComponent<EnemyTypeController>().MyEnemyType == EnemyType.Boss)//BossComment added to check if boss, temporary
            {
                return;
            }
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            freezeCount++;
            if (freezeCount <= freezeLimit)
                col.gameObject.GetComponent<EnemyConditionManager>().ChangeCondition(EnemyConditions.Frozen);
            else
            {
                col.gameObject.GetComponent<Enemyhp>().damage(damage, AttackType.Frost);
                col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
            }
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameAOEFinMeterFill);
            freezeCount++;
            if (freezeCount <= freezeLimit)
                col.gameObject.GetComponent<EnemyConditionManager>().ChangeCondition(EnemyConditions.Frozen);
            else
                col.gameObject.GetComponent<Enemyhp>().damage(damage, AttackType.Frost);
        }
    }
}
