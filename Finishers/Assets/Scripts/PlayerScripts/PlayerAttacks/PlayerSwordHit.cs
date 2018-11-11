using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordHit : MonoBehaviour {

    public FinisherMode finisherObject;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Enemy"))
        {
            finisherObject.IncreaseFinisherMeter();
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage();
            }
        }
        else if (col.gameObject.tag.Equals("TargetDummy"))
        {
            finisherObject.IncreaseFinisherMeter();
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage();
            }
        }

    }
}
