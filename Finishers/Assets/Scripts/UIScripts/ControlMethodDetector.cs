using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMethodDetector : MonoBehaviour {

    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;
    void Update()
    {
        string[] names = Input.GetJoystickNames();
        PS4_Controller = 0;
        Xbox_One_Controller = 0;
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;

            }
        }


        if (Xbox_One_Controller == 1)
        {
            GameStatus.CurrentControlType = ControlType.Xbox;
        }
        else if (PS4_Controller == 1)
        {
            GameStatus.CurrentControlType = ControlType.PS4;
        }
        else
        {
            GameStatus.CurrentControlType = ControlType.PC; 
        }
    }
}
