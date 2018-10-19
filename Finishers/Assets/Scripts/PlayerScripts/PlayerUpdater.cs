using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdater : MonoBehaviour {

    public CameraMovementController cameraMovement; 

    public void EnterCombatState()
    {
        cameraMovement.MoveToCombatLocation();
    }

    public void ExitCombatState()
    {
        cameraMovement.MoveToOOCLocation();
    }
}
