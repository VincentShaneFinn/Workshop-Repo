using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemyActions : MonoBehaviour {

    public EnemyMovementController MovementCtrl;
    public EnemyAI AI;
    public GameObject Sword;
    public GameObject HeavySword;
    private Transform playerT;

    public LayerMask Ground;
    public Transform _groundChecker;
    public float GroundDistance = 0f;

    void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    IEnumerator PerformNormalAttack()
    {
        MovementCtrl.StopMovement();
        AI.ChangeStatus(EnemyBehaviorStatus.Attacking);
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

    //Leap Attack for a knight
    IEnumerator PerformSpecial1Attack()
    {
        MovementCtrl.StopMovement();
        AI.ChangeStatus(EnemyBehaviorStatus.Attacking);
        AI.ChangeAction(EnemyActions.Special1);
        //set animation
        //attack()
        HeavySword.SetActive(true); //tempAniamtionFake
        GetComponent<EnemyMovementController>().DisableNavAgent();
        Vector3 dir = playerT.position - transform.position;
        GetComponent<Rigidbody>().AddForce(new Vector3(dir.x, 5, dir.z), ForceMode.VelocityChange);

        bool grounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        float tempAnimationTime = .2f; //use as a lift off time
        float tempAnimationCount = 0;

        //we need to end prematurely if the enemy is staggered
        while (!grounded || tempAnimationCount < tempAnimationTime)// && AI.CurrentStatus != EnemyBehaviorStatus.Staggered)
        {
            yield return null;
            grounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
            tempAnimationCount += Time.deltaTime;
        }

        HeavySword.SetActive(false);//tempAniamtionFake
        GetComponent<EnemyMovementController>().EnableNavAgent();
        AI.GetDirector().Special1AttackCompleted();
        AI.ChangeStatus(EnemyBehaviorStatus.Waiting);
        AI.ChangeAction(EnemyActions.None);
    }
}
