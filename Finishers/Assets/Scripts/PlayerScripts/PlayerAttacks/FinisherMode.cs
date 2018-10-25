﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FinisherModes { Runic, Siphoning, PressurePoints}

public class FinisherMode : MonoBehaviour {

    public GameObject Player;

    public float FinisherTime;
    private float FinisherCount;

    public float FinisherModeCooldownTime;
    private float FinisherModeCooldownCount;

    private FinisherModes CurrentFinisherMode;
    private bool inFinisherMode;
    private bool PerformingFinisher;
    private bool ExecutingFinisher;

    public PlayerSwordAttack swordController;
    public Transform EnemyFinisherPlacement;

    private GameObject currentTarget;

    public GameObject BlastBeam;
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public GameObject SlicedLimb;

    //Controls Slider UI
    public Slider finisherSlider;
    public int buildupVal = 20;

    //slow mo
    public float slowMoModifier;

    private CameraMovementController cam;

    void Start()
    {
        FinisherCount = FinisherTime;
        FinisherModeCooldownCount = FinisherModeCooldownTime;
        CurrentFinisherMode = FinisherModes.Runic;
        inFinisherMode = false;
        PerformingFinisher = false;
        ExecutingFinisher = false;
        cam = GameObject.FindGameObjectWithTag("CameraTarget").GetComponent<CameraMovementController>();
        finisherSlider = GameObject.Find("/Canvas/Sliders/Finisher Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {

        if (!inFinisherMode) { 
            if (FinisherModeCooldownCount >= FinisherModeCooldownTime)
            {
                if (Input.GetButtonDown("FinishMode") && !GameStatus.GamePaused && finisherSlider.value == 100)
                {
                    currentTarget = GetClosestEnemy();
                    if (currentTarget != null)
                    {
                        finisherSlider.value = 0;
                        StartCoroutine(EnterFinisherMode());
                    }
                    else
                    {
                        print("No nearby enemy");
                    }
                }
            }
            else
            {
                FinisherModeCooldownCount += Time.deltaTime;
            }

            //Increase UI slider for Finisher && temp cheat
            if (Input.GetKeyDown(KeyCode.G)) {
                IncreaseFinisherMeter();
            }
        }
        else{
            if (PerformingFinisher && !ExecutingFinisher)
            {
                if (Input.GetButtonDown("PrimaryAttack") && !GameStatus.GamePaused)
                {
                    CurrentFinisherMode = FinisherModes.Runic;
                    StartCoroutine(ExecuteFinisher());
                }
                else if (Input.GetButtonDown("SecondaryAttack") && !GameStatus.GamePaused)
                {
                    CurrentFinisherMode = FinisherModes.Siphoning;
                    print("Commit Siphoning Finisher");
                    StartCoroutine(ExecuteFinisher());
                }
                else if (Input.GetButtonDown("SpecialAttack") && !GameStatus.GamePaused)
                {
                    CurrentFinisherMode = FinisherModes.PressurePoints;
                    print("Commit Pressure Points Finisher");
                    StartCoroutine(ExecuteFinisher());
                }

                if (FinisherCount <= 0)
                {
                    FailFinisherMode();
                }
                else if (!GameStatus.GamePaused)
                {
                    FinisherCount -= Time.unscaledDeltaTime;
                }
            }
        }
        //print(NearbyEnemies);
    }

    public void IncreaseFinisherMeter()
    {
        finisherSlider.value += buildupVal;
    }
    
    IEnumerator EnterFinisherMode()
    {
        Player.GetComponent<PlayerMovementController>().PreventMoving();
        Player.GetComponent<PlayerMovementController>().PreventTuring();
        inFinisherMode = true;
        print("Begin Finisher");
        swordController.PreventAttacking();

        //move enemy into place and lock controls.
        currentTarget.transform.position = EnemyFinisherPlacement.position;
        currentTarget.transform.parent = EnemyFinisherPlacement;
        currentTarget.GetComponent<EnemyMovementController>().StopMovement();

        //moves camera
        cam.MoveToFinisherModeLocation();

        while (cam.GetIsMoving())
        {
            yield return null;
        }
        //waits till the camera and or animation is done

        PerformingFinisher = true;
        FinisherCount = FinisherTime;
        GameStatus.FinisherModeActive = true;
        Time.timeScale = slowMoModifier;
        
        //yield return null;
    }

    IEnumerator ExecuteFinisher()
    {
        PerformingFinisher = false;
        ExecutingFinisher = true;

        switch (CurrentFinisherMode)
        {
            case FinisherModes.Runic:
                Instantiate(BlastBeam, EnemyFinisherPlacement.position, EnemyFinisherPlacement.rotation);
                print("Commit Runit Finisher");
                break;
            case FinisherModes.Siphoning:
                GameObject part1 = Instantiate(TopHalf, new Vector3(currentTarget.transform.position.x, 1f, currentTarget.transform.position.z), currentTarget.transform.rotation);
                GameObject part2 = Instantiate(BottomHalf, new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z), currentTarget.transform.rotation);
                try { SlicedLimb.SetActive(true); } catch { }
                print("Commit Siphoning Finisher");
                break;
            case FinisherModes.PressurePoints:
                print("Commit PressurePoints Finisher");
                break;
            default:
                break;
        }
        currentTarget.GetComponent<EnemyAI>().KillEnemy();

        yield return null; // do stuff to perform the finisher
        StartCoroutine(LeavingFinisherMode());
    }

    public void FailFinisherMode()
    {
        PerformingFinisher = false;
        currentTarget.GetComponent<EnemyMovementController>().ResumeMovement();
        currentTarget.transform.parent = null;
        StartCoroutine(LeavingFinisherMode());
    }

    IEnumerator LeavingFinisherMode()
    {
        Player.GetComponent<PlayerMovementController>().AllowMoving();
        Player.GetComponent<PlayerMovementController>().AllowTurning();
        inFinisherMode = false;
        ExecutingFinisher = false;
        CurrentFinisherMode = FinisherModes.Runic;
        currentTarget = null;
        yield return null;
        swordController.ResumeAttacking();
        cam.MoveToCombatLocation();
        FinisherModeCooldownCount = 0;
        Time.timeScale = 1;
        GameStatus.FinisherModeActive = false;
    }


    public GameObject GetClosestEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject Enemy in Enemies)
        {
            if (Vector3.Distance(Enemy.transform.position, transform.position) < 5)
            {
                return Enemy;
            }
        }
        return null;
    }

}
