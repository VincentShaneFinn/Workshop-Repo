using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupDirector : MonoBehaviour {

    public List<GameObject> Exits;
    public GameObject EnemyGroupObject;

    private PlayerUpdater playerUpdater;
    private EnemyGroup Enemies;
    private bool CombatStarted;

	// Use this for initialization
	void Start () {
        CombatStarted = false;
        Enemies = new EnemyGroup();
        playerUpdater = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdater>();
        foreach (Transform child in EnemyGroupObject.transform)
        {
            if (child.CompareTag("Enemy")) {
                Enemies.AddEnemy(child.gameObject.GetComponent<EnemyAI>());
                child.gameObject.GetComponent<EnemyAI>().SetDirector(this);
            }
        }
    }


    float test = 0;
	// Update is called once per frame
	void Update () {
        if(Enemies.GetCount() <= 0)
        {
            //leave combat
            OpenExits();
            playerUpdater.ExitCombatState();
        }
        if (CombatStarted)
        {
            if (test == 0)
            {
            }
            if (test <= 3)
            {
                test += Time.deltaTime;
            }
            else
            {
                Enemies.AllEnemiesAttack();
            }
        }
	}

    void WakeUpEnemies()
    {
        Enemies.WakeUpEnemies();
    }

    //correctly removes the enemy from the list
    public void KillEnemy(EnemyAI enemy)
    {
        Enemies.RemoveEnemy(enemy);
    }



    void CloseExits()
    {
        foreach(GameObject exit in Exits)
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

    void OnTriggerEnter(Collider col)
    {
        WakeUpEnemies();

        //Make the enemies wait
        Enemies.AllEnemiesWait();

        //enter combat
        CloseExits();
        playerUpdater.EnterCombatState();

        gameObject.GetComponent<BoxCollider>().enabled = false;
        CombatStarted = true;
    }
}
