using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdater : MonoBehaviour {

    private CameraMovementController cameraMovement;

    public float ImmuneTime = .5f;
    public float ImmuneCount = .5f;
    public float PoiseTime = .5f;
    public float PoiseCount = .5f;

    void Start()
    {
        cameraMovement = GameObject.FindGameObjectWithTag("CameraTarget").GetComponent<CameraMovementController>();
    }

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
