using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeButtonIcon : MonoBehaviour {


    public GameObject PS4Icon;
    public GameObject PCIcon;

	// Use this for initialization
	void Start () {
        PS4Icon.SetActive(false);
        PCIcon.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if(GameStatus.CurrentControlType == ControlType.PC)
        {
            PS4Icon.SetActive(false);
            PCIcon.SetActive(true);
        }
        else if(GameStatus.CurrentControlType == ControlType.PS4)
        {
            PS4Icon.SetActive(true);
            PCIcon.SetActive(false);
        }
	}
}
