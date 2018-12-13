using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraPerpective : MonoBehaviour {

    public CameraMovementController cam;

	void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            cam.SwitchCombatLocation();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            cam.SwitchCombatLocation();
        }
    }
}
