using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyhp : MonoBehaviour {
    private int currenthp;
    public int hp =1;

	// Use this for initialization
	void Start () {
        currenthp = hp;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void checkhp() {
        print("ow");
        if (currenthp<=0) {
            if (gameObject.tag != "TargetDummy")
                GetComponent<EnemyAI>().KillEnemy();
            else
                Destroy(gameObject);
        }
    }
    public void damage() {
        currenthp--;
        checkhp();
    }
    public void damage(int d) {
        currenthp -= d;
        checkhp();
    }
}
