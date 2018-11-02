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
                print("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                print("XBOX ONE CONTROLLER IS CONNECTED");
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;

            }
        }


        if (Xbox_One_Controller == 1)
        {
            print("Xbox");
            GameStatus.CurrentControlType = ControlType.Xbox;
        }
        else if (PS4_Controller == 1)
        {
            print("PS4");
            GameStatus.CurrentControlType = ControlType.PS4;
        }
        else
        {
            GameStatus.CurrentControlType = ControlType.PC;
            print("Mouse and Keyboard");
        }
    }
}
