using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemyActions : MonoBehaviour {

    public EnemyMovementController MovementCtrl;
    public EnemyAI AI;
    public GameObject Sword;
    public GameObject HeavySword;
    private Transform playerT;

    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

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

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, playerT.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(playerT.position - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        HeavySword.SetActive(false);//tempAniamtionFake
        GetComponent<EnemyMovementController>().EnableNavAgent();
        AI.GetDirector().Special1AttackCompleted();
        AI.ChangeStatus(EnemyBehaviorStatus.Waiting);
        AI.ChangeAction(EnemyActions.None);
    }
}
