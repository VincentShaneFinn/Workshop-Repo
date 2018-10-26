using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void Start()
    {
        Enemies = new List<EnemyAI>(GetComponentsInChildren<EnemyAI>());
        playerUpdater = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdater>();
        myArcAngles = new ArcAngles();
    }

    public float SendOrderTime=0;
    private float SendOrderCounter=0;
    public void Update()
    {
        Enemies = Enemies.Where(item => item != null).ToList(); // remove killed enemies from list
        currentPrimaryAttackers = 0;
        currentArcRunners = 0;

        if (Enemies.Count <= 0)
        {
            OpenExits();
            playerUpdater.ExitCombatState();
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
        else {
            SendOrderCounter -= Time.deltaTime;
        }
    }
    //public GroupDirector() { Enemies = new List<EnemyAI>(); }

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

    public float TakeAngle() { return myArcAngles.TakeAngle(); }
    public void ReturnAngle(float returnAngle) { myArcAngles.ReturnAngle(returnAngle); }


    public void WakeUpEnemies()
    {
        foreach (EnemyAI enemy in Enemies)
        {
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
            //enemy.GetEnemyMovementCtrl().StopMovement();
            enemy.ChangeStatus(EnemyBehaviorStatus.Waiting);
        }
    }

    public void AllEnemiesAttack()
    {
        foreach (EnemyAI enemy in Enemies)
        {
            //enemy.GetEnemyMovementCtrl().ResumeMovement();
            enemy.ChangeStatus(EnemyBehaviorStatus.PrimaryAttacker);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        WakeUpEnemies();


        //enter combat
        CloseExits();
        playerUpdater.EnterCombatState();

        gameObject.GetComponent<BoxCollider>().enabled = false;
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
