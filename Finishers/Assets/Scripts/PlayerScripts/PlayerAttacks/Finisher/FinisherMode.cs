using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum FinisherModes { Runic, Siphoning, PressurePoints }
public enum Direction { up, right, down, left }

public class FinisherMode : MonoBehaviour
{

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

    [HideInInspector]public GameObject currentTarget;

    public GameObject BlastBeam;
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public GameObject SlicedLimb;
    public Transform SlicedLimbFirePoint;

    //Controls Slider UI
    public Slider finisherSlider;
    public int buildupVal = 20;
    public GameObject RunicRinisherGuides;
    public GameObject PrimaryAttackPopUp;

    //Animation Controller
    public Animator anim;

    //slow mo
    public float slowMoModifier;

    public CameraMovementController cam;
    public Transform CameraBase;
    public Transform PlayerRotWrapper;
    public bool TryFinisher = false;
    //private RunicInputHelper RunicSequence;

    private List<Direction> RunicQue;
    private List<FinisherAbstract> FinisherMoves = new List<FinisherAbstract>();
    public void AddFinisherMove(FinisherAbstract finisher)
    {
        FinisherMoves.Add(finisher);
    }

    void Start()
    {
        FinisherCount = FinisherTime;
        FinisherModeCooldownCount = FinisherModeCooldownTime;
        CurrentFinisherMode = FinisherModes.Runic;
        inFinisherMode = false;
        PerformingFinisher = false;
        ExecutingFinisher = false;

        FinisherIcon.gameObject.SetActive(true);
        FinisherIcon.SetActivated(false);
        InFinisherIcons.SetActive(false);
        PrimaryAttackPopUp.SetActive(false);
        //RunicSequence = new RunicInputHelper();
        RunicQue = new List<Direction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inFinisherMode)
        {
            if (FinisherModeCooldownCount >= FinisherModeCooldownTime)
            {
                if (finisherSlider.value >= 100)
                {
                    currentTarget = GetClosestEnemy();
                    if (TryFinisher && !GameStatus.GamePaused) //Input.GetButtonDown("FinishMode")
                    {
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
            }
            else
            {
                FinisherModeCooldownCount += Time.deltaTime;
            }

            //Increase UI slider for Finisher && temp cheat
            if (Input.GetKeyDown(KeyCode.G))
            {
                IncreaseFinisherMeter(100);
            }
        }
        else
        {
            CameraBase.rotation = PlayerRotWrapper.rotation;
            if (currentTarget != null)
            {
                currentTarget.transform.position = EnemyFinisherPlacement.position;
                currentTarget.transform.rotation = EnemyFinisherPlacement.rotation;
            }
            if (PerformingFinisher && !ExecutingFinisher && !GameStatus.GamePaused)
            {
                if (Input.GetButtonDown("UpButton"))
                {
                    anim.Play("RunicUpCarve");
                    RunicQue.Add(Direction.up);

                }
                if (Input.GetButtonDown("RightButton"))
                {
                    anim.Play("RunicRightCarve");
                    RunicQue.Add(Direction.right);
                }
                if (Input.GetButtonDown("DownButton"))
                {
                    anim.Play("RunicDownCarve");
                    RunicQue.Add(Direction.down);
                }
                if (Input.GetButtonDown("LeftButton"))
                {
                    anim.Play("RunicLeftCarve");
                    RunicQue.Add(Direction.left);
                }

                bool goodSoFar = false;
                foreach(FinisherAbstract RCombo in FinisherMoves)
                {
                    goodSoFar = RCombo.checkSoFar(RunicQue);
                    if (goodSoFar)
                    {
                        if (RCombo.check(RunicQue))
                            PrimaryAttackPopUp.SetActive(true);
                        break;
                    }
                }
                if (!goodSoFar)
                {
                    FailFinisherMode();
                    return;
                }

                //inside the primary attack check, see if they did a correct sequence, and succeed or fail
                if (Input.GetButtonDown("PrimaryAttack"))
                {
                    bool goodCombo = false;
                    foreach(FinisherAbstract f in FinisherMoves) {
                        goodCombo = f.startfinisher(this, RunicQue);
                        if (goodCombo)
                            break;
                    }
                    if (!goodCombo)
                    {
                        FailFinisherMode();
                    }
                    else {
                        StartCoroutine(ExecuteFinisher());
                    }
                }
                
                //if (Input.GetButtonDown("UpButton"))
                //{
                //    anim.Play("RunicUpCarve");
                //    if (!RunicSequence.AddInput(Direction.up))
                //    {
                //        FailFinisherMode();
                //        return;
                //    }

                //}
                //if (Input.GetButtonDown("RightButton"))
                //{
                //    anim.Play("RunicRightCarve");
                //    if (!RunicSequence.AddInput(Direction.right))
                //    {
                //        FailFinisherMode();
                //        return;
                //    }
                //}
                //if (Input.GetButtonDown("DownButton"))
                //{
                //    anim.Play("RunicDownCarve");
                //    if (!RunicSequence.AddInput(Direction.down))
                //    {
                //        FailFinisherMode();
                //        return;
                //    }
                //}
                //if (Input.GetButtonDown("LeftButton"))
                //{
                //    anim.Play("RunicLeftCarve");
                //    if (!RunicSequence.AddInput(Direction.left))
                //    {
                //        FailFinisherMode();
                //        return;
                //    }
                //}

                //if(RunicSequence.GetCount() == 3)
                //{
                //    PrimaryAttackPopUp.SetActive(true);
                //}
                //if(RunicSequence.GetCount() > 3)
                //{
                //    FailFinisherMode();
                //    return;
                //}

                ////inside the primary attack check, see if they did a correct sequence, and succeed or fail
                //if (Input.GetButtonDown("PrimaryAttack"))
                //{
                //    if (RunicSequence.SuccessfulCombo(RunicSequence.FireCombo))
                //    {
                //        CurrentFinisherMode = FinisherModes.Runic;
                //        StartCoroutine(ExecuteFinisher());
                //    }
                //    else if (RunicSequence.SuccessfulCombo(RunicSequence.IceCombo))
                //    {
                //        CurrentFinisherMode = FinisherModes.Siphoning;
                //        print("Commit Siphoning Finisher");
                //        StartCoroutine(ExecuteFinisher());
                //    }
                //    else
                //        FailFinisherMode();
                //}
                //else if (Input.GetButtonDown("SpecialAttack"))
                //{
                //    CurrentFinisherMode = FinisherModes.PressurePoints;
                //    print("Commit Pressure Points Finisher");
                //    StartCoroutine(ExecuteFinisher());
                //}

                if (FinisherCount <= 0)
                {
                    FailFinisherMode();
                }
                else
                {
                    FinisherCount -= Time.unscaledDeltaTime;
                }
            }
        }
        //print(NearbyEnemies);
        TryFinisher = false;
    }

    public void IncreaseFinisherMeter()
    {
        finisherSlider.value += buildupVal;
    }
    public void IncreaseFinisherMeter(int val)
    {
        finisherSlider.value += val;
    }

    public IEnumerator EnterFinisherMode()
    {
        inFinisherMode = true;
        print("Begin Finisher");
        swordController.PreventAttacking();

        //move enemy into place and lock controls.
        currentTarget.transform.position = EnemyFinisherPlacement.position;
        currentTarget.transform.parent = EnemyFinisherPlacement;

        if (currentTarget.tag != "TargetDummy")
        {
            currentTarget.GetComponent<EnemyMovementController>().StopMovement();
            currentTarget.GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.BeingFinished);
        }

        anim.Play("FinisherRunicIdleStance");
        yield return null;
        Player.GetComponent<PlayerMovementController>().PreventMoving();
        Player.GetComponent<PlayerMovementController>().PreventTuring();

        //moves camera
        cam.MoveToFinisherModeLocation(); //Mark make sure camera takes as long as the animation

        while (cam.GetIsMoving())
        {
            CameraBase.rotation = Quaternion.Slerp(CameraBase.rotation, PlayerRotWrapper.rotation, .5f);
            yield return null;
        }
        //waits till the camera and or animation is done
        CameraBase.rotation = PlayerRotWrapper.rotation;

        PerformingFinisher = true;
        FinisherCount = FinisherTime;
        GameStatus.FinisherModeActive = true;
        Time.timeScale = slowMoModifier;

        FinisherIcon.SetActivated(false);
        InFinisherIcons.SetActive(true);
        RunicRinisherGuides.SetActive(true);

        //RunicSequence.RestartQue();
        RunicQue.Clear();
        //yield return null;
    }

    IEnumerator ExecuteFinisher()
    {
        PerformingFinisher = false;
        ExecutingFinisher = true;

        PrimaryAttackPopUp.SetActive(false);
        RunicRinisherGuides.SetActive(false);
        InFinisherIcons.SetActive(false);

        //switch (CurrentFinisherMode)
        //{
            
        //    case FinisherModes.Runic:
        //        Vector3 rot = EnemyFinisherPlacement.rotation.eulerAngles;
        //        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        //        GameObject beam = Instantiate(BlastBeam, EnemyFinisherPlacement.position, Quaternion.Euler(rot));
        //        beam.transform.parent = PlayerRotWrapper;
        //        print("Commit Runit Finisher");
        //        break;
        //    case FinisherModes.Siphoning:
        //        GameObject part1 = Instantiate(TopHalf, new Vector3(currentTarget.transform.position.x, 1f, currentTarget.transform.position.z), currentTarget.transform.rotation);
        //        GameObject part2 = Instantiate(BottomHalf, new Vector3(currentTarget.transform.position.x, 0f, currentTarget.transform.position.z), currentTarget.transform.rotation);
        //        try { Instantiate(SlicedLimb, SlicedLimbFirePoint); } catch { }
        //        GetComponent<PlayerHealthController>().PlayerHealed(20);
        //        anim.Play("SlashL");
        //        print("Commit Siphoning Finisher");
        //        break;
        //    case FinisherModes.PressurePoints:
        //        print("Commit PressurePoints Finisher");
        //        break;
        //    default:
        //        break;
                
        //}

        yield return null; // do stuff to perform the finisher
        StartCoroutine(LeavingFinisherMode());
    }

    public void FailFinisherMode()
    {
        PerformingFinisher = false;

        PrimaryAttackPopUp.SetActive(false);
        RunicRinisherGuides.SetActive(false);
        InFinisherIcons.SetActive(false);
        anim.Play("idle");

        StartCoroutine(LeavingFinisherMode());
    }

    IEnumerator LeavingFinisherMode()
    {
        Player.GetComponent<PlayerMovementController>().AllowMoving(); //MARK: barely noticable bug where if you move and do flamethrower finisher, you may move for 1 frame
        Player.GetComponent<PlayerMovementController>().AllowTurning(); 

        if (currentTarget.tag != "TargetDummy")
            currentTarget.GetComponent<EnemyAI>().KillEnemy();
        else
            Destroy(currentTarget);

        inFinisherMode = false;
        ExecutingFinisher = false;
        CurrentFinisherMode = FinisherModes.Runic;
        currentTarget = null;
        yield return null;
        swordController.ResumeAttacking();
        if (GameStatus.InCombat)
            cam.MoveToCombatLocation();
        else
            cam.MoveToOOCLocation();
        FinisherModeCooldownCount = 0;
        Time.timeScale = 1;
        GameStatus.FinisherModeActive = false;
    }

    public ChangeButtonIcon FinisherIcon;
    public GameObject InFinisherIcons;
    public GameObject GetClosestEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(Enemies.Length <= 0)
            FinisherIcon.SetActivated(false);

        foreach (GameObject Enemy in Enemies)
        {
            if (Vector3.Distance(Enemy.transform.position, transform.position) < 5 && Enemy.GetComponent<NavMeshAgent>().isActiveAndEnabled)
            {
                FinisherIcon.SetActivated(true);
                FinisherIcon.transform.position = Enemy.transform.position;
                return Enemy;
            }
            else
            {
                FinisherIcon.SetActivated(false);
            }
        }
        GameObject[] TargetDummies = GameObject.FindGameObjectsWithTag("TargetDummy");
        foreach (GameObject dummy in TargetDummies)
        {
            if (Vector3.Distance(dummy.transform.position, transform.position) < 5)
            {
                FinisherIcon.SetActivated(true);
                FinisherIcon.transform.position = dummy.transform.position;
                return dummy;
            }
            else
            {
                FinisherIcon.SetActivated(false);
            }
        }

        return null;
    }

}
