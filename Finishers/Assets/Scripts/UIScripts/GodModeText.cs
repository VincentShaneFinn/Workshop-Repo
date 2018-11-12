using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodModeText : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if(GameStatus.CurrentControlType == ControlType.PC)
        {
            GetComponent<Text>().text = "Q";
        }
        else
        {
            GetComponent<Text>().text = "R1";
        }
	}
}
