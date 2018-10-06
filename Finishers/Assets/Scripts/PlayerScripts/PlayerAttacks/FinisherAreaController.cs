using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherAreaController : MonoBehaviour {

    public FinisherMode fm;
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //fm.SetEnemy(col.gameObject);
        }
    }
}
