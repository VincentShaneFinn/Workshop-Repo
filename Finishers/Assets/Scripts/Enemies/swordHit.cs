using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordHit : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            Destroy(col.gameObject);
        }
    }
}
