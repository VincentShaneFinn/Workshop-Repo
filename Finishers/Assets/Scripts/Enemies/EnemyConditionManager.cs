using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyConditions { Normal, OnFire, Cold, Frozen}
public class EnemyConditionManager : MonoBehaviour {

    public GameObject Icecycle;
    public EnemyConditions CurrentCondition;
    private EnemyMovementController emc;
    private EnemyAI ai;
    private EnemyStaggerController esc;

    // Use this for initialization
    void Start () {
		emc = GetComponent<EnemyMovementController>();
        ai = GetComponent<EnemyAI>();
        esc = GetComponent<EnemyStaggerController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeCondition(EnemyConditions condi)
    {
        if (CurrentCondition != condi)
        {
            CurrentCondition = condi;
        }
        switch (condi)
        {
            case EnemyConditions.Frozen:
                StartCoroutine(FreezeEnemy());
                break;
            case EnemyConditions.Normal:
                //StartCoroutine(ReturnToNormal());
                ReturnToNormal();
                break;
        }
    }

    //Additional changes done in enemyhp when the enemy is frozen
    IEnumerator FreezeEnemy()
    {
        Icecycle.SetActive(true);
        if (emc != null)
            emc.StopMovement();
        if (ai != null)
        {
            ai.ChangeStatus(EnemyBehaviorStatus.Frozen);
            ai.anim.Play("Death");
            ai.anim.speed = 0;
        }
        yield return new WaitForSeconds(PlayerDamageValues.Instance.FrostTimeToMelt);
        Icecycle.SetActive(false);
        if (emc != null)
            emc.ResumeMovement();
        if (ai != null)
        {
            ai.ChangeStatus(EnemyBehaviorStatus.Waiting);
            ai.anim.speed = 1;
            ai.anim.Play("Idle");
        }

    }

    void ReturnToNormal()
    {

    }
}
