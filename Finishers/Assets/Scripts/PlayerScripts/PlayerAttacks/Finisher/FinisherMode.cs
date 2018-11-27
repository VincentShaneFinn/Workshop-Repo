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

    public GameObject TopHalf;
    public GameObject BottomHalf;
    public GameObject SlicedLimb;
    public Transform SlicedLimbFirePoint;

    //Controls Slider UI
    public GameObject FinisherFullImage;
    public Slider finisherSlider;
    public Slider GodModeSlider;
    public int buildupVal = 20;
    public GameObject RunicRinisherGuides;
    public GameObject PrimaryAttackPopUp;

    //Animation Controller
    public Animator UIanim;
    public Animator CharAnim;

    //slow mo
    public float slowMoModifier;

    public CameraMovementController cam;
    public Transform CameraBase;
    public Transform PlayerRotWrapper;
    public bool TryFinisher = false;
    public bool CanFinish = true;
    //private RunicInputHelper RunicSequence;

    public PlayerSoundController psc;

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

        FinisherFullImage.SetActive(false);
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
            if (FinisherModeCooldownCount >= FinisherModeCooldownTime && CanFinish)
            {
                if (finisherSlider.value >= 100)
                {
                    currentTarget = GetClosestEnemy();
                    FinisherFullImage.SetActive(true);
                    if (TryFinisher && !GameStatus.GamePaused) //Input.GetButtonDown("FinishMode")
                    {
                        if (currentTarget != null)
                        {
                            finisherSlider.value = 50;
                            FinisherFullImage.SetActive(false);
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
                    FinisherFullImage.SetActive(false);
                }
            }
            else
            {
                FinisherModeCooldownCount += Time.deltaTime;
                FinisherIcon.SetActivated(false);
            }

            //Increase UI slider for Finisher && temp cheat
            if (Input.GetKeyDown(KeyCode.G))
            {
                IncreaseFinisherMeter(100);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                IncreaseGodModeMeter(100);
            }
        }
        else
        {
            CameraBase.rotation = PlayerRotWrapper.rotation;

            if (PerformingFinisher && !ExecutingFinisher && !GameStatus.GamePaused) //MARK: Unsure of how we will do the full finisher carves
            {
                currentTarget.transform.position = EnemyFinisherPlacement.position;
                currentTarget.transform.rotation = EnemyFinisherPlacement.rotation;
                if (Input.GetButtonDown("QuickFinish"))
                {
                    FailFinisherMode();
                    return;
                }
                if (Input.GetButtonDown("UpButton"))
                {
                    UIanim.Play("RunicUpCarve");
                    CharAnim.Play("Carve 1");
                    RunicQue.Add(Direction.up);
                    psc.PlayRunicStab(Direction.up);
                }
                if (Input.GetButtonDown("RightButton"))
                {
                    UIanim.Play("RunicRightCarve");
                    RunicQue.Add(Direction.right);
                    psc.PlayRunicStab(Direction.right);
                }
                if (Input.GetButtonDown("DownButton"))
                {
                    UIanim.Play("RunicDownCarve");
                    CharAnim.Play("Carve 2");
                    RunicQue.Add(Direction.down);
                    psc.PlayRunicStab(Direction.down);
                }
                if (Input.GetButtonDown("LeftButton"))
                {
                    UIanim.Play("RunicLeftCarve");
                    RunicQue.Add(Direction.left);
                    psc.PlayRunicStab(Direction.left);
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
                if (Input.GetButtonDown("Execute"))
                {
                    bool goodCombo = false;
                    FinisherAbstract FinisherToPerform = null;
                    foreach (FinisherAbstract f in FinisherMoves) {
                        //goodCombo = f.startfinisher(this, RunicQue);
                        goodCombo = f.check(RunicQue);
                        FinisherToPerform = f;
                        if (goodCombo)
                            break;
                    }
                    if (!goodCombo)
                    {
                        FailFinisherMode();
                    }
                    else {
                        StartCoroutine(ExecuteFinisher(FinisherToPerform));
                    }
                }

                if (FinisherCount <= 0 && !ExecutingFinisher)
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

    public void IncreaseFinisherMeter(float val)
    {
        finisherSlider.value += val;
    }
    public void IncreaseGodModeMeter(float val)
    {
        GodModeSlider.value += val;
    }

    public GameObject SwordThrowAnimObj;
    public IEnumerator EnterFinisherMode()
    {
        GameStatus.FinisherModeActive = true;
        Player.GetComponent<PlayerMovementController>().PreventMoving();
        Player.GetComponent<PlayerMovementController>().PreventTuring();
        inFinisherMode = true;
        print("Begin Finisher");
        swordController.PreventAttacking();

        //move enemy into place and lock controls.

        PlayerRotWrapper.LookAt(new Vector3(currentTarget.transform.position.x,
                                PlayerRotWrapper.transform.position.y,
                                currentTarget.transform.position.z));

        if (currentTarget.tag != "TargetDummy")
        {
            currentTarget.GetComponent<EnemyMovementController>().StopMovement();
            currentTarget.GetComponent<EnemyAI>().BeingFinished();
            yield return null;
        }

        UIanim.Play("FinisherRunicIdleStance");

        Time.timeScale = slowMoModifier;

        if (usedSwordGrapple)
        {
            //moves camera
            cam.MoveToAimingLocation(true); //Mark make sure camera takes as long as the animation

            usedSwordGrapple = false;

            //move sword forward and back over a small period
            SwordThrowAnimObj.SetActive(true);
            var savedSwordPos = SwordThrowAnimObj.transform.localPosition;
            var savedSwordRot = SwordThrowAnimObj.transform.localRotation;
            var currentSwordPos = SwordThrowAnimObj.transform.position;
            Vector3 FinalTarget = EnemyFinisherPlacement.position;
            float timeToMove = .25f;
            var t = 0f;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime / timeToMove;
                SwordThrowAnimObj.transform.position = Vector3.Lerp(currentSwordPos, currentTarget.transform.position, t);
                yield return null;
            }
            currentSwordPos = SwordThrowAnimObj.transform.position;
            var currentTargetPos = currentTarget.transform.position;
            t = 0f;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime / timeToMove;
                SwordThrowAnimObj.transform.position = Vector3.Lerp(currentSwordPos, FinalTarget, t);
                currentTarget.transform.position = Vector3.Lerp(currentTargetPos, FinalTarget, t);
                yield return null;
            }
            SwordThrowAnimObj.transform.localPosition = savedSwordPos;
            SwordThrowAnimObj.transform.localRotation = savedSwordRot;
            SwordThrowAnimObj.SetActive(false);
            CharAnim.Play("FinisherStart");
            yield return null;
        }
        else
        {
            CharAnim.Play("FinisherStart");
            yield return null;
        }

        currentTarget.transform.position = EnemyFinisherPlacement.position;
        currentTarget.transform.rotation = EnemyFinisherPlacement.rotation;
        currentTarget.transform.parent = EnemyFinisherPlacement;

        //moves camera
        cam.MoveToFinisherModeLocation(); //Mark make sure camera takes as long as the animation

        while (cam.GetIsMoving())
        {
            CameraBase.rotation = Quaternion.Slerp(CameraBase.rotation, PlayerRotWrapper.rotation, .5f);
            yield return null;
        }
        //waits till the camera and or animation is done
        CameraBase.rotation = PlayerRotWrapper.rotation;
        GetComponent<PlayerMovementController>().Aiming = false;

        PerformingFinisher = true;
        FinisherCount = FinisherTime;

        FinisherIcon.SetActivated(false);
        InFinisherIcons.SetActive(true);
        RunicRinisherGuides.SetActive(true);

        //RunicSequence.RestartQue();
        RunicQue.Clear();
        //yield return null;
    }

    IEnumerator ExecuteFinisher(FinisherAbstract FinisherToPerform)
    {
        PerformingFinisher = false;
        ExecutingFinisher = true;

        PrimaryAttackPopUp.SetActive(false);
        RunicRinisherGuides.SetActive(false);
        InFinisherIcons.SetActive(false);

        finisherSlider.value = 0;
        CharAnim.Play("FinisherExecution");
        yield return new WaitForSecondsRealtime(1f);
        FinisherToPerform.startfinisher(this);

        yield return null; // do stuff to perform the finisher

        IncreaseGodModeMeter(PlayerDamageValues.Instance.ExecuteFinisherGMFill);
        StartCoroutine(LeavingFinisherMode());
    }

    private bool didFail = false;
    public void FailFinisherMode()
    {
        PerformingFinisher = false;

        PrimaryAttackPopUp.SetActive(false);
        RunicRinisherGuides.SetActive(false);
        InFinisherIcons.SetActive(false);
        didFail = true;

        StartCoroutine(LeavingFinisherMode());
    }

    IEnumerator LeavingFinisherMode()
    {
        UIanim.Play("idle");
        if (didFail)
        {
            CharAnim.Play("FinisherExecution");
            yield return new WaitForSecondsRealtime(1f);
            CharAnim.Play("Idle");
            didFail = false;
        }
        Player.GetComponent<PlayerMovementController>().AllowMoving(); //MARK: barely noticable bug where if you move and do flamethrower finisher, you may move for 1 frame
        Player.GetComponent<PlayerMovementController>().AllowTurning();


        if (currentTarget.tag != "TargetDummy")
            currentTarget.GetComponent<EnemyAI>().KillEnemy();
        else
            Destroy(currentTarget);

        if(currentTarget != null)
        {
            currentTarget.transform.parent = null;
            if (currentTarget.GetComponent<EnemyAI>() != null)
            {
                currentTarget.GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.PrimaryAttacker);
                currentTarget.GetComponent<EnemyMovementController>().HelpKnockback();
            }
        }

        inFinisherMode = false;
        ExecutingFinisher = false;
        CurrentFinisherMode = FinisherModes.Runic;
        currentTarget = null;
        yield return null;
        swordController.ResumeAttacking();
        if (GameStatus.InCombat)
            cam.SwitchCombatLocation();
        else
            cam.SwitchCombatLocation();
        FinisherModeCooldownCount = 0;
        Time.timeScale = 1;
        GameStatus.FinisherModeActive = false;
    }

    public ChangeButtonIcon FinisherIcon;
    public GameObject InFinisherIcons;
    public SiphonHolsterController shc;
    public float GrappleFinishRange = 30;
    private bool usedSwordGrapple = false;

    public GameObject GetClosestEnemy()
    {
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(Enemies.Length <= 0)
            FinisherIcon.SetActivated(false);

        float range = 5;
        if (shc.HasSword())
            range = GrappleFinishRange;

        GameObject thisCurrentTarget = null;
        float lowestDistance = Mathf.Infinity;
        GameObject thisCurrentDotTarget = null;
        float lowestDotDistance = Mathf.Infinity;
        foreach (GameObject Enemy in Enemies)
        {
            if (Vector3.Distance(Enemy.transform.position, transform.position) < range)
            {
                //check if the player is in front of you
                var heading = Enemy.transform.position - CameraBase.position;
                float dot = Mathf.Abs(Vector3.Angle(CameraBase.forward, heading));
                if (dot < 30) // must be 30 degrees in front
                {
                    if (heading.magnitude < lowestDistance)
                    {
                        thisCurrentTarget = Enemy;
                        lowestDistance = heading.magnitude;
                    }
                    if (dot < lowestDotDistance)
                    {
                        thisCurrentDotTarget = Enemy;
                        lowestDotDistance = dot;
                    }
                }
            }
        }
        if (!GameStatus.InCombat)
        {
            GameObject[] TargetDummies = GameObject.FindGameObjectsWithTag("TargetDummy");
            foreach (GameObject dummy in TargetDummies)
            {
                if (Vector3.Distance(dummy.transform.position, transform.position) < range)
                {
                    //check if the player is in front of you
                    var heading = dummy.transform.position - CameraBase.position;
                    float dot = Mathf.Abs(Vector3.Angle(CameraBase.forward, heading));
                    if (dot < 30) // must be 30 degrees in front
                    {
                        if (heading.magnitude < lowestDistance)
                        {
                            thisCurrentTarget = dummy;
                            lowestDistance = heading.magnitude;
                        }
                        if (dot < lowestDotDistance)
                        {
                            thisCurrentDotTarget = dummy;
                            lowestDotDistance = dot;
                        }
                    }
                }
            }
        }

        if (thisCurrentTarget != null)
        {
            FinisherIcon.SetActivated(true);
            if (Vector3.Distance(thisCurrentDotTarget.transform.position,this.transform.position) > 5)
            {
                FinisherIcon.transform.position = thisCurrentDotTarget.transform.position;
                usedSwordGrapple = true;
                return thisCurrentDotTarget;
            }
            FinisherIcon.transform.position = thisCurrentTarget.transform.position;
            return thisCurrentTarget;
        }
        
        FinisherIcon.SetActivated(false);
        return null;
    }

}
