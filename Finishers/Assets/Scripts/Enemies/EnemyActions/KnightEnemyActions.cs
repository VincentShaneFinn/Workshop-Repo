using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemyActions : MonoBehaviour {

    public EnemyMovementController MovementCtrl;
    public EnemyAI AI;
    public GameObject Sword;

    IEnumerator PerformNormalAttack()
    {
        //If two many attacks recently, continue doing what they were doing
        if (!AI.GetDirector().TryNormalAttack())
            yield break;

        MovementCtrl.StopMovement();
        AI.ChangeStatus(EnemyBehaviorStatus.Busy);
        AI.ChangeAction(EnemyActions.NormalAttack);
        //set animation
        //attack()
        Sword.SetActive(true); //tempAniamtionFake

        float tempAnimationTime = 1;
        float tempAnimationCount = 0;

        //we need to end prematurely if the enemy is staggered
        while(tempAnimationCount < tempAnimationTime && AI.CurrentStatus != EnemyBehaviorStatus.Staggered)
        {
            yield return null;
            tempAnimationCount += Time.deltaTime;
        }

        Sword.SetActive(false);//tempAniamtionFake

        AI.GetDirector().NormalAttackCompleted();
        AI.ChangeStatus(EnemyBehaviorStatus.Waiting);
        AI.ChangeAction(EnemyActions.None);
    }
}
