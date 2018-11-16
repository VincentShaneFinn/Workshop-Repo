using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//BUSY is used as a generic for the enemy doing something else and should be skipped by director assigning roles
public enum EnemyBehaviorStatus { Sleeping, PrimaryAttacker, ArcRunner, SurroundPlayer, Attacking, Busy, Waiting, Staggered, BeingFinished, Dying }
//Staggered will currently be used for when the enemy is interupted, it will let the director know that it failed to do its task,
//this way the director can recalculate if necessary
//WE MAY WANT TO CHANGE THIS TO A CLASS SO SOMETHING LIKE AMBUSY CAN BE IN ONE A SMART PLACE

public class GroupDirector : MonoBehaviour{

    public List<GameObject> Exits;
    private PlayerUpdater playerUpdater;

    private List<EnemyAI> Enemies;

    //RoleVariables
    public int MaxPrimaryAttackers = 1;
    private int currentPrimaryAttackers;
    public int MaxArcRunners = 2;
    private int currentArcRunners;

    private ArcAngles myArcAngles;
    private ActionManager myActionManager;
    public float ReturnNormalAttackDelay = 1f;
    public float ReturnSpecial1AttackDelay = 5f;
    public int MaxAttackActions = 2;
    public int MaxNormalAttacks = 2;
    public int MaxSpecial1Attacks = 1;
    public float SendOrderTime = 1;
    private float SendOrderCounter = 0;
    private bool CombatStarted = false;
    public void Start()
    {
        Enemies = new List<EnemyAI>(GetComponentsInChildren<EnemyAI>());
        playerUpdater = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdater>();
        myArcAngles = new ArcAngles();
        myActionManager = new ActionManager();
        myActionManager.MaxAttackActions = MaxAttackActions;
        myActionManager.MaxNormalAttacks = MaxNormalAttacks;
        myActionManager.MaxSpecial1Attacks = MaxSpecial1Attacks;
        foreach(GameObject door in Exits)
        {
            door.SetActive(false);
        }
    }
    public void Update()
    {
        if (CombatStarted)
        {
            Enemies = Enemies.Where(item => item != null).ToList(); // remove killed enemies from list
            currentPrimaryAttackers = 0;
            currentArcRunners = 0;

            if (Enemies.Count <= 0)
            {
                OpenExits();
                playerUpdater.ExitCombatState();
                gameObject.SetActive(false);
            }

            if (SendOrderCounter < 0)
            {
                //SortEnemies by remainingdistance To player
                Enemies.Sort(delegate (EnemyAI a, EnemyAI b)
                {
                //using remaining distance doesn't work correctly when using arc angles
                return Vector3.Distance(a.GetEnemyMovementController().transform.position, playerUpdater.transform.position)
                    .CompareTo(
                      Vector3.Distance(b.GetEnemyMovementController().transform.position, playerUpdater.transform.position));
                });

                //Temporary? check if an enemy is in an attack state, and add to primary and arc runner counts since we dont need anyone else, could do the same for staggered
                foreach (EnemyAI enemy in Enemies)
                {
                    if (enemy.GetCurrentStatus() == EnemyBehaviorStatus.Attacking)
                    {
                        if (currentPrimaryAttackers < MaxPrimaryAttackers)
                        {
                            currentPrimaryAttackers++;
                        }
                        else if (currentArcRunners < MaxArcRunners)
                        {
                            currentArcRunners++;
                        }
                    }
                }


                //Hand out roles, give closest enemies primary attacker, followed by arc runners, then surrounders
                foreach (EnemyAI enemy in Enemies)
                {
                    if (enemy.GetCurrentStatus() == EnemyBehaviorStatus.PrimaryAttacker || enemy.GetCurrentStatus() == EnemyBehaviorStatus.ArcRunner || enemy.GetCurrentStatus() == EnemyBehaviorStatus.SurroundPlayer || enemy.GetCurrentStatus() == EnemyBehaviorStatus.Waiting)
                    {
                        if (currentPrimaryAttackers < MaxPrimaryAttackers) // start by giving out primary attackers
                        {
                            enemy.ChangeStatus(EnemyBehaviorStatus.PrimaryAttacker);
                            currentPrimaryAttackers++;
                        }
                        else if (currentArcRunners < MaxArcRunners) // then give the arc runners
                        {
                            enemy.ChangeStatus(EnemyBehaviorStatus.ArcRunner);
                            currentArcRunners++;
                        }
                        else
                            enemy.ChangeStatus(EnemyBehaviorStatus.SurroundPlayer);
                    }
                }

                SendOrderCounter = SendOrderTime;
            }
            else
            {
                SendOrderCounter -= Time.deltaTime;
            }
        }
    }

    //Check if a status is busy or not, this check if the given status is something that should not be interupted
    public bool IsBusy(EnemyBehaviorStatus status)
    {
        if (status != EnemyBehaviorStatus.PrimaryAttacker && status != EnemyBehaviorStatus.ArcRunner && status != EnemyBehaviorStatus.SurroundPlayer && status != EnemyBehaviorStatus.Waiting)
            return true;
        return false;
    }

    //Access list of enemies
    public void AddEnemy(EnemyAI e)
    {
        Enemies.Add(e);
    }
    public void RemoveEnemy(EnemyAI e)
    {
        Enemies.Remove(e);
    }

    public int GetCount()
    {
        return Enemies.Count;
    }

    //Access ArcAngles
    public float TakeAngle() { return myArcAngles.TakeAngle(); }
    public void ReturnAngle(float returnAngle) { myArcAngles.ReturnAngle(returnAngle); }

    //Access ActionManager
    //Normal take and return
    public bool TryNormalAttack() {
        //print("BeforeTaking" + myActionManager.CurrentNormalAttacks);
        return myActionManager.TryNormalAttack();
    }
    public void NormalAttackCompleted() {
        StartCoroutine(ExecuteAfterTime(ReturnNormalAttackDelay, () => myActionManager.NormalAttackCompleted()));
    }

    //Special1 take and return
    public bool TrySpecial1Attack()
    {
        //print("BeforeTaking" + myActionManager.CurrentNormalAttacks);
        return myActionManager.TrySpecial1Attack();
    }
    public void Special1AttackCompleted()
    {
        StartCoroutine(ExecuteAfterTime(ReturnSpecial1AttackDelay, () => myActionManager.Special1AttackCompleted()));
    }

    //Used to delay Returning attacks to ActionManager
    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
        //print("After Returning" + myActionManager.CurrentNormalAttacks);
    }

    public void WakeUpEnemies()
    {
        foreach (EnemyAI enemy in Enemies)
        {
            if (enemy != null)
                enemy.gameObject.GetComponent<EnemyAI>().wakeup();
        }
    }

    //set the n closest enemies as the attackers
    public void SetAttackers(int n)
    {

    }

    public void AllEnemiesWait()
    {
        foreach(EnemyAI enemy in Enemies)
        {
            if (enemy != null)
                enemy.ChangeStatus(EnemyBehaviorStatus.Waiting);
        }
    }

    public void AllEnemiesAttack()
    {
        foreach (EnemyAI enemy in Enemies)
        {
            if (enemy != null)
                //enemy.GetEnemyMovementCtrl().ResumeMovement();
                enemy.ChangeStatus(EnemyBehaviorStatus.PrimaryAttacker);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            WakeUpEnemies();


            //enter combat
            CloseExits();
            playerUpdater.EnterCombatState();

            gameObject.GetComponent<BoxCollider>().enabled = false;
            CombatStarted = true;
        }
    }

    void CloseExits()
    {
        foreach (GameObject exit in Exits)
        {
            exit.SetActive(true);
        }
    }

    void OpenExits()
    {
        foreach (GameObject exit in Exits)
        {
            exit.SetActive(false);
        }
    }

}
