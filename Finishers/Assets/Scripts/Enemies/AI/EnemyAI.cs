using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyActions { None, NormalAttack, Special1 }

public class EnemyAI : MonoBehaviour {
    public float keepPlayerDistance;
    public float sidespeed;
    public float attackrange = 1;
    public KnightEnemyActions KnightActions;
    public EnemyMovementController GetEnemyMovementCtrl;
    private GroupDirector director;
    private EnemyActions myAction;
    //private EnemyBehaviorStatus UpdatedStatus = EnemyBehaviorStatus.Sleeping; //this is updated by the director using enemygroup
    public EnemyBehaviorStatus CurrentStatus = EnemyBehaviorStatus.Sleeping; //this is what the current status is, and stuff should be done if UpdatedStatus changes
    public EnemyBehaviorStatus PreviousStatus;

    private Transform playerT;
    public float Special1RangeTEMP = 5;
    public Animator anim;

    private float ArcAngle = 360; //use 360 as a sub for null
    private Vector3 ArcTarget;
    // Use this for initialization
    void Start() {
        GetEnemyMovementCtrl = GetComponent<EnemyMovementController>();
        SetDirector(GetComponentInParent<GroupDirector>());
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        myAction = EnemyActions.None;
    }

    private float StaggeredCheckTime = .7f;
    private float StaggeredCheckCount = 0;
    public void ResetStaggeredCheck() { StaggeredCheckCount = 0; }

    // Update is called once per frame
    void Update () {
        //Animation Section Start
        if (anim != null)
        {
            anim.SetFloat("EnemyMoving", GetEnemyMovementCtrl.agent.speed);
        }
        //Animation Section End

        //MARK: TEMPORARY FIX FOR ENEMIES REMAINING STAGGERED
        if (CurrentStatus == EnemyBehaviorStatus.Staggered)
        {
            StaggeredCheckCount += Time.deltaTime;
            if (StaggeredCheckCount >= StaggeredCheckTime)
            {
                ChangeStatus(EnemyBehaviorStatus.Waiting);
                anim.Play("Idle");
            }
        }
        else
        {
            StaggeredCheckCount = 0;
        }

        UpdateEnemyBehaviorStatus();
        if(ArcAngle != 360 && CurrentStatus != EnemyBehaviorStatus.ArcRunner)
        {
            director.ReturnAngle(ArcAngle);
            ArcAngle = 360;
            GetEnemyMovementCtrl.SetTarget(playerT);
        }

        //we will need to make some new moves if it is a boss, currently preventing boss from jumping

        //obviosuly if your currently doing something, or shouldnt be able to do something, you can't attack
        if (!director.IsBusy(CurrentStatus) && myAction == EnemyActions.None) {
            if (checkplayer(attackrange))
            {
                //check if the player is in front of you
                var heading = playerT.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);
                if (dot > .5) // must be 30 degrees in front
                {
                    //If two many attacks recently, continue doing what they were doing
                    if (director.TryNormalAttack())
                        KnightActions.StartCoroutine("PerformNormalAttack");
                    else
                        KeepDistance();
                }
            }
            //This will need to go to KnightEnemyActions and check which attack it should attempt, rather than using range
            else if (checkplayer(Special1RangeTEMP) && Vector3.Distance(transform.position, playerT.position) > Special1RangeTEMP - 1 && etc.MyEnemyType != EnemyType.Boss) // we will need an alternative way to check if doing special 1 is right that is specific to the enemy
            {
                if (director.TrySpecial1Attack())
                    KnightActions.StartCoroutine("PerformSpecial1Attack");
            }
            else if (CurrentStatus == EnemyBehaviorStatus.SurroundPlayer && Vector3.Distance(transform.position, playerT.position) > ProjectileLaunchDistance)
            {
                if (CanThrow)
                {
                    if (director.TryProjectileAttack())
                    {
                        CanThrow = false;
                        StartCoroutine(KnightActions.ThrowProjectile());
                    }
                }
            }
        }
        if(CurrentStatus == EnemyBehaviorStatus.Busy && CanThrowAOE) //BossComment
        {
            StartCoroutine(KnightActions.ThrowFireAOE());
        }
        if(StartupCheck && CurrentStatus != EnemyBehaviorStatus.Sleeping) {
            startupTime += Time.deltaTime;
            if(startupTime > director.ReturnProjectileAttackDelay)
            {
                CanThrow = true;
                StartupCheck = false;
            }
        }
    }
    public float ProjectileLaunchDistance = 10;
    private bool StartupCheck = true;
    public bool CanThrow = false;
    private float startupTime = 0;

    public EnemyBehaviorStatus GetCurrentStatus() { return CurrentStatus; }
    public void ChangeStatus(EnemyBehaviorStatus s) { PreviousStatus = CurrentStatus;  CurrentStatus = s; }

    //crucial to returning action so a dead guy doesn't hold on to it forever, may want a check in the director
    public void ChangeAction(EnemyActions act){ myAction = act; }

    public EnemyMovementController GetEnemyMovementController(){ return GetEnemyMovementCtrl; }

    //in this future this will probably need to be a coroutine, becuase it might take a few seconds to change status and update animation
    //and once that is complete, start up whatever the next status is.
    public void UpdateEnemyBehaviorStatus()
    {
        anim.SetBool("TempSurround", false);
        switch (CurrentStatus)
        {
            case EnemyBehaviorStatus.PrimaryAttacker:
                GetEnemyMovementCtrl.ResumeMovement();
                break;
            case EnemyBehaviorStatus.ArcRunner:
                GetEnemyMovementCtrl.ResumeMovement();
                TravelAlongArc();
                break;
            case EnemyBehaviorStatus.SurroundPlayer:
                KeepDistance();
                break;
            case EnemyBehaviorStatus.Waiting:
                GetEnemyMovementCtrl.StopMovement();
                break;
            case EnemyBehaviorStatus.Attacking:
                break;
            case EnemyBehaviorStatus.Busy:
                break;
            case EnemyBehaviorStatus.Sleeping:
                GetEnemyMovementCtrl.StopMovement();
                break;
            case EnemyBehaviorStatus.Staggered:
                GetEnemyMovementCtrl.StopMovement();
                break;
            case EnemyBehaviorStatus.BeingFinished:
                GetEnemyMovementCtrl.StopMovement();
                break;
            default:
                GetEnemyMovementCtrl.StopMovement();
                break;
        }
    }

    private void KeepDistance()
    {
        if (checkplayer(keepPlayerDistance))
        {
            GetEnemyMovementCtrl.StopMovement();
            transform.LookAt(playerT); //this is meant to look at the player right now
            //Debug.Log(dx+","+x);
            Collider[] c = Physics.OverlapSphere(transform.position, keepPlayerDistance * 1.7f);
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
            if (Mathf.Abs(dx) < 0.3 || x == 0)
            {
                //guard
                //StartCoroutine("GuardState");
                anim.SetBool("TempSurround", false);
            }
            else
            {
                transform.Translate(Vector3.left * dx / x * Time.deltaTime * sidespeed);
                anim.SetBool("TempSurround", true);
            }
        }
        else
        {
            GetEnemyMovementCtrl.ResumeMovement();
        }
    }

    public void TravelAlongArc()
    {
        if (ArcAngle == 360)
        {
            ArcAngle = director.TakeAngle();
        }

        Vector3 midpoint = (transform.position + playerT.transform.position) / 2f;
        float x = midpoint.x + (transform.position.x - midpoint.x) * Mathf.Cos(ArcAngle) - (transform.position.z - midpoint.z) * Mathf.Sin(ArcAngle);
        float z = midpoint.z + (transform.position.x - midpoint.x) * Mathf.Sin(ArcAngle) + (transform.position.z - midpoint.z) * Mathf.Cos(ArcAngle);
        //float x = playerT.position.x + (transform.position.x - playerT.position.x) * Mathf.Cos(ArcAngle) - (transform.position.z - playerT.position.z) * Mathf.Sin(ArcAngle);
        //float z = playerT.position.z + (transform.position.x - playerT.position.x) * Mathf.Sin(ArcAngle) + (transform.position.z - playerT.position.z) * Mathf.Cos(ArcAngle);

        //do angle side angle calculation from the distance to player

        //test changing cube positions, or we should add ray trace lines
        ArcTarget = new Vector3(x, transform.position.y, z);
        Debug.DrawLine(transform.position, ArcTarget);
        if (GetEnemyMovementCtrl.GetRemainingDistance() < 3)
            GetEnemyMovementCtrl.SetDestination(playerT.position);
        else
            GetEnemyMovementCtrl.SetDestination(ArcTarget);
        GetEnemyMovementCtrl.ResumeMovement();
    }

    //returns true if within a distance to the player
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

    public int finishersToKill = 1; //BossComment stuff
    private int timesKilled = 0;
    public int GetTimesKilled() { return timesKilled; }
    public EnemyTypeController etc;
    public Transform FountainTop;
    public bool CanThrowAOE = true ;
    //THIS IS THE ONLY WAY AN ENEMY SHOULD BE KILLED
    public void KillEnemy()
    {
        if (ArcAngle != 360 && CurrentStatus != EnemyBehaviorStatus.ArcRunner)
        {
            director.ReturnAngle(ArcAngle);
            ArcAngle = 360;
        }
        if(myAction != EnemyActions.None)
            switch (myAction)
            {
                case EnemyActions.NormalAttack:
                    director.NormalAttackCompleted();
                    break;
                case EnemyActions.Special1:
                    director.Special1AttackCompleted();
                    break;
                default:
                    break;
            }

        timesKilled++;
        if (timesKilled >= finishersToKill)
            Destroy(gameObject);

        //BossComment specific code
        if (etc.MyEnemyType == EnemyType.Boss && (float)(finishersToKill - GetTimesKilled()) / finishersToKill < .34f)
        {
            etc.EnemySkin.material = GetComponent<Enemyhp>().lowRed;
            GetEnemyMovementCtrl.ResumeMovement();
            GetEnemyMovementCtrl.SetLockToGround(true);
            GetComponent<EnemyMovementController>().EnableNavAgent();
        }
        else if(etc.MyEnemyType == EnemyType.Boss && (float)(finishersToKill - GetTimesKilled()) / finishersToKill < .68f)
        {
            StartCoroutine(PutOnFountain());
        }
    }
    IEnumerator PutOnFountain()
    {
        yield return new WaitForSeconds(1);
        ChangeStatus(EnemyBehaviorStatus.Busy);
        GetEnemyMovementCtrl.StopMovement();
        GetEnemyMovementCtrl.SetLockToGround(false);
        GetComponent<EnemyMovementController>().DisableNavAgent();
        yield return null;
        transform.position = FountainTop.position;
    }

    public void BeingFinished()
    {
        ChangeStatus(EnemyBehaviorStatus.BeingFinished);
        anim.Play("Hit4");
    }

    public void wakeup() {
        if (anim == null)
            CurrentStatus = EnemyBehaviorStatus.Waiting;
        else
            StartCoroutine(WakeUpAnimate());
    }

    IEnumerator WakeUpAnimate()
    {
        anim.SetFloat("SleepModifier", 1);
        yield return new WaitForSeconds(3.4f);
        if (!director.IsInterupted(CurrentStatus))
        {
            anim.Play("Idle");
            CurrentStatus = EnemyBehaviorStatus.Waiting;
        }
        //Animation Section End
    }

    public GroupDirector GetDirector()
    {
        return director;
    }
    public void SetDirector(GroupDirector d)
    {
        if(d != null)
            director = d;
    }

    //IEnumerator GuardState() {
    //    GetEnemyMovementCtrl.StopMovement();
    //    CurrentStatus = EnemyBehaviorStatus.Busy;
    //    //set animation
    //    //guard()
    //    yield return new WaitForSeconds(0.3f);//animation time

    //    CurrentStatus = EnemyBehaviorStatus.Waiting;
    //}

}
