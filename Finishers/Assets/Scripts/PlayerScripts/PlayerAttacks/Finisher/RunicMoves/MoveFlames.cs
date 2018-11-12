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
        if (state.Equals("Finished"))
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
            col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
        }
        else if(col.gameObject.tag == "TargetDummy")
        {
            ehp.damage(1);
        }
    }
}
