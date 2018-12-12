using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BossComment whole class is temp
public class TempEnemySpawn : MonoBehaviour {

    public GameObject EnemyToSpawn;
    private GameObject myEnemy;
    public GroupDirector director;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (director.GetCombatStarted())
        {
            if (myEnemy == null && !called)
            {
                called = true;
                Invoke("SpawnEnemy", 4);  
            }
        }
	}

    bool called = false;

    void SpawnEnemy()
    {
        myEnemy = Instantiate(EnemyToSpawn, transform.position, transform.rotation);
        myEnemy.transform.parent = director.transform;
        myEnemy.GetComponent<EnemyAI>().SetDirector(director);
        myEnemy.gameObject.GetComponent<EnemyAI>().wakeup();
        called = false;
    }
}
