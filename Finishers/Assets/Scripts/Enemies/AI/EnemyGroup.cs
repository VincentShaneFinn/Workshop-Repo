using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGroup:MonoBehaviour{

    public List<GameObject> Exits;
    private PlayerUpdater playerUpdater;

    private List<EnemyAI> Enemies;
    public void Start()
    {
        Enemies = new List<EnemyAI>(GetComponentsInChildren<EnemyAI>());
        playerUpdater = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdater>();
    }

    public float attackrate=10;
    private float attackcounter=0;
    public void Update()
    {
        Enemies = Enemies.Where(item => item != null).ToList(); // remove killed enemies from list
        if (Enemies.Count <= 0)
        {
            OpenExits();
            playerUpdater.ExitCombatState();
        }

        if (attackcounter < 0)
        {
            List<EnemyAI> t = new List<EnemyAI>();
            foreach (EnemyAI e in Enemies)
            {
                if (e.CurrentStatus.Equals(EnemyBehaviorStatus.Sleeping))
                {
                }
                if (e.CurrentStatus.Equals(EnemyBehaviorStatus.Waiting))
                {
                    t.Add(e);
                }
            }
            if (t.Count > 0)
            {
                int i = 0;
                i = Random.Range(0, t.Count);
                t[i].SetToAttack();
                attackcounter = attackrate;
            }
        }
        else {
            attackcounter -= Time.deltaTime;
        }
    }
    public EnemyGroup() { Enemies = new List<EnemyAI>(); }

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
            enemy.ChangeStatus(EnemyBehaviorStatus.Attacking);
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
