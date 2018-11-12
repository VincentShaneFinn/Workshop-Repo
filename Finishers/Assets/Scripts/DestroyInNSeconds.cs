using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInNSeconds : MonoBehaviour {


    public float DestroyTime;
    private float DestroyCount;

    void Start()
    {
        DestroyCount = DestroyTime;
    }

    void Update()
    {
        DestroyCount -= Time.deltaTime;
        if(DestroyCount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<Enemyhp>().damage(3);
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            col.gameObject.GetComponent<Enemyhp>().damage(3);
        }
    }

}
