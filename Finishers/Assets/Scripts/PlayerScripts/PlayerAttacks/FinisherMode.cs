using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FinisherModes { Runic, Siphoning, PressurePoints}

public class FinisherMode : MonoBehaviour {

    public GameObject Player;

    public float FinisherTime;
    private float FinisherCount;

    public float FinisherModeTime;
    private float FinisherModeCount;

    private FinisherModes CurrentFinisherMode;
    private bool inFinisherMode;
    private bool PerfromingFinisher;

    public SwordAttack swordController;
    public Transform EnemyFinisherPlacement;

    private GameObject currentTarget;

    public GameObject BlastBeam;
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public GameObject SlicedLimb;

    private CameraMovementController cam;

    void Start()
    {
        FinisherCount = FinisherTime;
        FinisherModeCount = FinisherModeTime;
        CurrentFinisherMode = FinisherModes.Runic;
        inFinisherMode = false;
        PerfromingFinisher = false;
        cam = Camera.main.GetComponent<CameraMovementController>();
    }

    // Update is called once per frame
    void Update() {
        if (FinisherCount >= FinisherTime)
        {
            if (Input.GetKeyDown(KeyCode.M) && !Cursor.visible)
            {
                FinisherCount = 0;
                //Time.timeScale = .1f;
            }
        }
        else
        {
            FinisherCount += Time.deltaTime;
        }

        if (!inFinisherMode) { 
            if (FinisherModeCount >= FinisherModeTime)
            {
                if (Input.GetKeyDown(KeyCode.F) && !Cursor.visible)
                {
                    currentTarget = GetClosestEnemy();
                    if (currentTarget != null)
                    {
                        EnterFinisherMode();
                    }
                    else
                    {
                        print("No nearby enemy");
                    }
                }
            }
            else
            {
                FinisherModeCount += Time.deltaTime;
            }
        }
        else{
            if (!PerfromingFinisher && !cam.GetIsMoving())
            {
                if (Input.GetMouseButtonDown(0) && !Cursor.visible)
                {
                    CurrentFinisherMode = FinisherModes.Runic;
                    StartCoroutine(PerformFinisher());
                }
                else if (Input.GetMouseButtonDown(1) && !Cursor.visible)
                {
                    CurrentFinisherMode = FinisherModes.Siphoning;
                    print("Commit Siphoning Finisher");
                    StartCoroutine(PerformFinisher());
                }
                else if (Input.GetMouseButtonDown(2) && !Cursor.visible)
                {
                    CurrentFinisherMode = FinisherModes.PressurePoints;
                    print("Commit Pressure Points Finisher");
                    StartCoroutine(PerformFinisher());
                }
            }
        }
        //print(NearbyEnemies);
    }


    
    //IEnumerator EnterFinisherMode()
    public void EnterFinisherMode()
    {
        Player.GetComponent<PlayerMovementController>().PreventMoving();
        FinisherModeCount = 0;
        inFinisherMode = true;
        print("Begin Finisher");
        swordController.PreventAttacking();

        //move enemy into place and lock controls.
        currentTarget.transform.position = EnemyFinisherPlacement.position;
        currentTarget.transform.parent = EnemyFinisherPlacement;
        currentTarget.GetComponent<EnemyMovementController>().StopMovement();

        cam.MoveToFinisherModeLocation();

        //yield return null;
    }

    IEnumerator PerformFinisher()
    {
        PerfromingFinisher = true;

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
        Destroy(currentTarget);

        yield return null; // do stuff to perform the finisher
        StartCoroutine(LeavingFinisherMode());
    } 

    IEnumerator LeavingFinisherMode()
    {
        Player.GetComponent<PlayerMovementController>().AllowMoving();
        inFinisherMode = false;
        PerfromingFinisher = false;
        CurrentFinisherMode = FinisherModes.Runic;
        currentTarget = null;
        yield return null;
        swordController.ResumeAttacking();
        cam.MoveToCombatLocation();
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
