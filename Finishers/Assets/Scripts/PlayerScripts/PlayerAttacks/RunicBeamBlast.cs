using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicBeamBlast : MonoBehaviour {

    public GameObject parent;
    public float DestroyTime;
    private float DestroyCount;
    public PlayerMovementController pmc;
    
	// Use this for initialization
	void Start () {
        DestroyCount = DestroyTime;
        pmc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
	}
	
	// Update is called once per frame
	void Update () {
        DestroyCount -= Time.deltaTime;
        pmc.rotationSpeed = 1; //MARK: temporary slow player;
        pmc.CanMove = false;
        if (DestroyCount <= 0)
        {
            pmc.CanMove = true;
            pmc.rotationSpeed = 40;
            Destroy(parent);
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyMovementController>().HelpKnockback();
            col.gameObject.GetComponent<Enemyhp>().damage(1,true);
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            col.gameObject.GetComponent<Enemyhp>().damage(1,true);
        }
    }
}
