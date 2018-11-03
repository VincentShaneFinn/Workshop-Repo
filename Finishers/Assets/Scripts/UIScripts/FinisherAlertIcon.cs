using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherAlertIcon : MonoBehaviour {

    public GameObject PCIcon;
    public GameObject PS4Icon;
    private GameObject Cam;

	// Use this for initialization
	void Awake () {
        PCIcon.SetActive(false);
        PS4Icon.SetActive(false);
        Cam = Camera.main.gameObject;
	}
	
	void FixedUpdate () {
		if(GameStatus.CurrentControlType == ControlType.PC)
        {
            PCIcon.SetActive(true);
            PS4Icon.SetActive(false);
        }
        else if(GameStatus.CurrentControlType == ControlType.PS4)
        {
            PCIcon.SetActive(false);
            PS4Icon.SetActive(true);
        }

        Vector3 targetPostition = new Vector3(Cam.transform.position.x,
                                        this.transform.position.y,
                                        Cam.transform.position.z);
        this.transform.LookAt(targetPostition);
	}
}
