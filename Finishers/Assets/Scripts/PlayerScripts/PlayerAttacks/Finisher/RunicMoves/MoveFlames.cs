using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlames : MonoBehaviour {

    public GameObject parent;
    private Animator anim;
    private float damage;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        damage = PlayerDamageValues.Instance.FlamethrowerWaveDamage;
    }

    void Update()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Finished"))
        {
            Destroy(parent);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Enemyhp ehp = col.gameObject.GetComponent<Enemyhp>();
        if(col.gameObject.tag == "Enemy")
        {
            ehp.damage(damage, AttackType.Fire);
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameThrowFinMeterFill);
            col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
        }
        else if(col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter(PlayerDamageValues.Instance.FlameThrowFinMeterFill);
            ehp.damage(damage, AttackType.Fire);
        }
    }
}
