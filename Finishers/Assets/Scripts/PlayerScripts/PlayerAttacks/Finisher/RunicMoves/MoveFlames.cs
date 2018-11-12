using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlames : MonoBehaviour {

    public GameObject parent;
    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
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
            ehp.damage(1);
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter();
            col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
        }
        else if(col.gameObject.tag == "TargetDummy")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>().IncreaseFinisherMeter();
            ehp.damage(1);
        }
    }
}
