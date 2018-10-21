using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdater : MonoBehaviour {

    public CameraMovementController cameraMovement; 

    public void EnterCombatState()
    {
        GameStatus.InCombat = true;
        cameraMovement.MoveToCombatLocation();
    }

    public void ExitCombatState()
    {
        GameStatus.InCombat = false;
        cameraMovement.MoveToOOCLocation();
    }
}
