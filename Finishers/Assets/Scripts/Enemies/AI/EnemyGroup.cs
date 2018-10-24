using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup {

    private List<EnemyAI> Enemies;


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
            enemy.gameObject.SetActive(true);
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

}
