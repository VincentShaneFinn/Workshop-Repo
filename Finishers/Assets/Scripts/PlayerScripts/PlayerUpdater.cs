using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdater : MonoBehaviour {

    public CameraMovementController cameraMovement;

    public float ImmuneTime = .5f;
    public float ImmuneCount = .5f;
    public float PoiseTime = .5f;
    public float PoiseCount = .5f;

    void Update()
    {
        ImmuneCount += Time.deltaTime;
        PoiseCount += Time.deltaTime;
    }

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
