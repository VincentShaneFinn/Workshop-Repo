using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordRange : MonoBehaviour {

    public GameObject EnemySword;
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "PlayerModel")
        {
            EnemySword.SetActive(true);
        }
    }
}
