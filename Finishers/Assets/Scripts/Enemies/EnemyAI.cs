using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviorStatus { Sleeping, Waiting, Attacking, Staggered, ReadyForOrder, Dead }
//Staggered will currently be used for when the enemy is interupted, it will let the director know that it failed to do its task,
//this way the director can recalculate if necessary

public class EnemyAI : MonoBehaviour {
    public GameObject hixbox;
    public float keepPlayerDistance;
    public float sidespeed;
    public float attackrange=1;
    private GroupDirector Director;
    public bool attacking;
    //private EnemyBehaviorStatus UpdatedStatus = EnemyBehaviorStatus.Sleeping; //this is updated by the director using enemygroup
    public EnemyBehaviorStatus CurrentStatus = EnemyBehaviorStatus.Sleeping; //this is what the current status is, and stuff should be done if UpdatedStatus changes
    //might have things like WaitingToAttacking as a current status, which represents the intermission time needed to switch from one
    //status to another

    // Use this for initialization
    void Start () {
        if (GetEnemyMovementCtrl == null) {
            GetEnemyMovementCtrl = GetComponent<EnemyMovementController>();
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		//NOTE
        //I am still unsure if the enemygroup class should used by the director to manually update each enemy
        //or simply update the Status variable, and in each enemies update method, change what it is doing if the status has changed

        //I wanted to put this when ChangeStatus was called, but it doesnt make the enemies move for some reason
       // if(CurrentStatus != UpdatedStatus)
        {
            UpdateToCurrentStatus();
        }
	}

    //public EnemyAI GetEnemyAI() { return GetComponent<EnemyAI>(); }
    //public EnemyMovementController GetEnemyMovementCtrl() { return GetComponent<EnemyMovementController>(); }
    private EnemyMovementController GetEnemyMovementCtrl=null;

    public EnemyBehaviorStatus GetCurrentStatus() { return CurrentStatus; }
    //public void ChangeStatus(EnemyBehaviorStatus s) { UpdatedStatus = s; }
    public void ChangeStatus(EnemyBehaviorStatus s) { CurrentStatus = s; }

    //in this future this will probably need to be a coroutine, becuase it might take a few seconds to change status and update animation
    //and once that is complete, start up whatever the next status is.
    public void UpdateToCurrentStatus()
    {
       // CurrentStatus = UpdatedStatus;
        switch (CurrentStatus)
        {
            case EnemyBehaviorStatus.Waiting:

                
                if (checkplayer(keepPlayerDistance))
                {
                    if (attacking)
                    {
                        if (checkplayer(attackrange))
                        {
                            //normalattack
                            StartCoroutine("AttackState");
                            break;
                        }
                        else
                        {
                            GetEnemyMovementCtrl.ResumeMovement();
                            break;
                        }
                    }
                    GetEnemyMovementCtrl.StopMovement();
                    transform.LookAt(GetEnemyMovementCtrl.Target);
                    //Debug.Log(dx+","+x);
                    Collider[] c = Physics.OverlapSphere(transform.position, keepPlayerDistance*1.7f);
                    float x = 0.01f;
                    float dx = 0;
                    foreach (Collider col in c)
                    {
                        if (col.tag.Equals("Enemy"))
                        {
                            if (!col.transform.Equals(transform))
                            {
                                float d = keepPlayerDistance * 1.7f / Vector3.Distance(transform.position, col.transform.position) * transform.InverseTransformPoint(col.transform.position).x;
                                dx += d;
                                x += Mathf.Abs(d);
                            }
                        }
                    }
                        if (Mathf.Abs(dx) < 0.3 || x==0)
                    {
                        //guard
                        StartCoroutine("GuardState");
                    }
                    else
                    {
                        transform.Translate(Vector3.left * dx / x * Time.deltaTime * sidespeed);
                    }
                }
                else
                {
                    if (attacking) {
                        if (checkplayer(attackrange)) {
                            //sprintattack
                            StartCoroutine("AttackState");
                            break;
                        }
                    }
                    GetEnemyMovementCtrl.ResumeMovement();
                }
                break;

            case EnemyBehaviorStatus.Attacking:
                GetEnemyMovementCtrl.StopMovement();
                break;
            case EnemyBehaviorStatus.Sleeping:
                GetEnemyMovementCtrl.StopMovement();
                break;
            default:
                break;
        }
    }
    private bool checkplayer(float radius) {
        Collider[] c = Physics.OverlapSphere(transform.position, radius);
        bool a = false;

        foreach (Collider col in c)
        {
            if (col.tag.Equals("Player"))
            {
                a = true;
            }
        }
        return a;
    }
    public void SetDirector(GroupDirector d)
    {
        Director = d;
    }

    //THIS IS THE ONLY WAY AN ENEMY SHOULD BE KILLED
    public void KillEnemy()
    {
        //Director.KillEnemy(this);
        Destroy(gameObject);
    }

    public void SetToAttack()
    {
        attacking = true;
    }

    public void SetToWait()
    {
        attacking = false;
    }
    public void wakeup() {
        CurrentStatus = EnemyBehaviorStatus.Waiting;
    }
    IEnumerator AttackState()
    {
        GetEnemyMovementCtrl.StopMovement();
        CurrentStatus = EnemyBehaviorStatus.Attacking;
        //set animation
        //attack()
        GameObject h = Instantiate(hixbox,transform);
        h.transform.Translate(Vector3.forward);
        
        
        yield return new WaitForSeconds(0.5f);//animation time/attack time
        CurrentStatus = EnemyBehaviorStatus.Waiting;
        SetToWait();
        
    }
    IEnumerator GuardState() {
        GetEnemyMovementCtrl.StopMovement();
        CurrentStatus = EnemyBehaviorStatus.Attacking;
        //set animation
        //guard()
        yield return new WaitForSeconds(0.3f);//animation time

        CurrentStatus = EnemyBehaviorStatus.Waiting;
    }

}
