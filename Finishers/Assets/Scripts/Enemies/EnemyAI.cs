using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviorStatus { Sleeping, Waiting, Attacking, Dead }

public class EnemyAI : MonoBehaviour {

    private GroupDirector Director;
    private EnemyBehaviorStatus UpdatedStatus; //this is updated by the director using enemygroup
    private EnemyBehaviorStatus CurrentStatus; //this is what the current status is, and stuff should be done if UpdatedStatus changes
    //might have things like WaitingToAttacking as a current status, which represents the intermission time needed to switch from one
    //status to another

    // Use this for initialization
    void Start () {
        UpdatedStatus = EnemyBehaviorStatus.Sleeping;
        CurrentStatus = EnemyBehaviorStatus.Sleeping;
    }
	
	// Update is called once per frame
	void Update () {
		//NOTE
        //I am still unsure if the enemygroup class should used by the director to manually update each enemy
        //or simply update the Status variable, and in each enemies update method, change what it is doing if the status has changed

        if(CurrentStatus != UpdatedStatus)
        {
            UpdateToCurrentStatus();
        }
	}

    public EnemyAI GetEnemyAI() { return GetComponent<EnemyAI>(); }
    public EnemyMovementController GetEnemyMovementCtrl() { return GetComponent<EnemyMovementController>(); }

    public EnemyBehaviorStatus GetCurrentStatus() { return CurrentStatus; }
    public void ChangeStatus(EnemyBehaviorStatus s) { UpdatedStatus = s; }

    //in this future this will probably need to be a coroutine, becuase it might take a few seconds to change status and update animation
    //and once that is complete, start up whatever the next status is.
    public void UpdateToCurrentStatus()
    {
        CurrentStatus = UpdatedStatus;
        switch (CurrentStatus)
        {
            case EnemyBehaviorStatus.Waiting:
                GetEnemyMovementCtrl().StopMovement();
                break;
            case EnemyBehaviorStatus.Attacking:
                GetEnemyMovementCtrl().ResumeMovement();
                break;
            default:
                break;
        }
    }

    public void SetDirector(GroupDirector d)
    {
        Director = d;
    }

    //THIS IS THE ONLY WAY AN ENEMY SHOULD BE KILLED
    public void KillEnemy()
    {
        Director.KillEnemy(this);
        Destroy(gameObject);
    }

    public void SetToAttack()
    {
        gameObject.GetComponent<EnemyMovementController>().ResumeMovement();
    }

    public void SetToWait()
    {
        gameObject.GetComponent<EnemyMovementController>().StopMovement();
    }
}
