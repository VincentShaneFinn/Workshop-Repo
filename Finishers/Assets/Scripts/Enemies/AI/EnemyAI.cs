﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviorStatus { Sleeping, PrimaryAttacker, ArcRunner, SurroundPlayer, Busy, Waiting, Staggered, BeingFinished, Dying }
//Staggered will currently be used for when the enemy is interupted, it will let the director know that it failed to do its task,
//this way the director can recalculate if necessary

public class EnemyAI : MonoBehaviour {
    public GameObject EnemySword;
    public float keepPlayerDistance;
    public float sidespeed;
    public float attackrange = 1;
    private EnemyMovementController GetEnemyMovementCtrl;
    private GroupDirector director;
    //private EnemyBehaviorStatus UpdatedStatus = EnemyBehaviorStatus.Sleeping; //this is updated by the director using enemygroup
    public EnemyBehaviorStatus CurrentStatus = EnemyBehaviorStatus.Sleeping; //this is what the current status is, and stuff should be done if UpdatedStatus changes
    private Transform playerT;

    private float ArcAngle = 360; //use 360 as a sub for null
    private Vector3 ArcTarget;
    // Use this for initialization
    void Start () {
        GetEnemyMovementCtrl = GetComponent<EnemyMovementController>();
        director = GetComponentInParent<GroupDirector>();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateEnemyBehaviorStatus();
        if(ArcAngle != 360 && CurrentStatus != EnemyBehaviorStatus.ArcRunner)
        {
            director.ReturnAngle(ArcAngle);
            ArcAngle = 360;
            GetEnemyMovementCtrl.SetTarget(playerT);
        }

        if (checkplayer(attackrange))
        {
            //sprintattack
            StartCoroutine("AttackState");
        }
    }

    public EnemyBehaviorStatus GetCurrentStatus() { return CurrentStatus; }
    public void ChangeStatus(EnemyBehaviorStatus s) { CurrentStatus = s; }

    public EnemyMovementController GetEnemyMovementController(){ return GetEnemyMovementCtrl; }

    //in this future this will probably need to be a coroutine, becuase it might take a few seconds to change status and update animation
    //and once that is complete, start up whatever the next status is.
    public void UpdateEnemyBehaviorStatus()
    {
       // CurrentStatus = UpdatedStatus;
        switch (CurrentStatus)
        {
            case EnemyBehaviorStatus.PrimaryAttacker:
                GetEnemyMovementCtrl.ResumeMovement();
                break;
            case EnemyBehaviorStatus.ArcRunner:
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
                GetEnemyMovementCtrl.SetDestination(ArcTarget);
                GetEnemyMovementCtrl.ResumeMovement();
                break;
            case EnemyBehaviorStatus.SurroundPlayer:
                KeepDistance();
                break;
            case EnemyBehaviorStatus.Waiting:
                GetEnemyMovementCtrl.StopMovement();
                break;
            case EnemyBehaviorStatus.Busy:
                GetEnemyMovementCtrl.StopMovement();
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
            }
            else
            {
                transform.Translate(Vector3.left * dx / x * Time.deltaTime * sidespeed);
            }
        }
        else
        {
            GetEnemyMovementCtrl.ResumeMovement();
        }
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

    //THIS IS THE ONLY WAY AN ENEMY SHOULD BE KILLED
    public void KillEnemy()
    {
        if (ArcAngle != 360 && CurrentStatus != EnemyBehaviorStatus.ArcRunner)
        {
            director.ReturnAngle(ArcAngle);
            ArcAngle = 360;
        }
        Destroy(gameObject);
    }

    public void wakeup() {
        CurrentStatus = EnemyBehaviorStatus.Waiting;
    }

    IEnumerator AttackState()
    {
        GetEnemyMovementCtrl.StopMovement();
        CurrentStatus = EnemyBehaviorStatus.Busy;
        //set animation
        //attack()
        EnemySword.SetActive(true);


        yield return new WaitForSeconds(2f);//animation time/attack time
        CurrentStatus = EnemyBehaviorStatus.Waiting;

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
