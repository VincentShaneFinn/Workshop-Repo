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
    public void RemoveFinisherMove(FinisherAbstract finisher)
    {
        FinisherMoves.Remove(finisher);
    }
    public List<FinisherAbstract> GetUnlockedFinishers()
    {
        return FinisherMoves;
    }
    public void SetUnlockedFinishers(List<FinisherAbstract> fa)
    {
        FinisherMoves = fa;
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

    //TutorialPillarComment
    public bool PillarFinisherNearby = false;
    public GameObject PillarTarget = null;
    public bool PillarFinisherUsed = false;
    private TutorialPillar PillarTutorial;

    void HidePopup()
    {
        TutorialPopups.Instance.HideTutorialPopup();
    }
    void SaveGame()
    {
        gameStatus.SaveGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inFinisherMode)
        {
            if (FinisherModeCooldownCount >= FinisherModeCooldownTime && CanFinish)
            {
                if (PillarFinisherNearby && PillarTarget != null)//TutorialPillarComment
                {
                    FinisherIcon.SetActivated(true);
                    FinisherIcon.transform.position = PillarTarget.transform.position;
                    FinisherIcon.transform.position += Vector3.up * 2f;
                    if (TryFinisher && !GameStatus.GamePaused)
                    {
                        PillarFinisherUsed = true;
                        currentTarget = PillarTarget;
                        PillarTutorial = PillarTarget.GetComponent<TutorialPillar>();
                        switch (PillarTutorial.FinisherUnlock)
                        {
                            case Finishers.Siphoning:
                                GetComponent<Siphoncut>().enabled = true;
                                break;
                            case Finishers.FlameSword:
                                GetComponent<RunicFireSword>().enabled = true;
                                break;
                            case Finishers.Flamethrower:
                                GetComponent<RunicFlamethrower>().enabled = true;
                                break;
                            case Finishers.FlameAOE:
                                GetComponent<RunicFireCircle>().enabled = true;
                                break;
                            case Finishers.FrostAOE:
                                GetComponent<RunicFrostCircle>().enabled = true;
                                break;
                        }
                        StartCoroutine(EnterFinisherMode(false));
                    }
                }
                else if (finisherSlider.value >= 100)
                {
                    currentTarget = GetClosestEnemy();
                    FinisherFullImage.SetActive(true);
                    if (TryFinisher && !GameStatus.GamePaused) //Input.GetButtonDown("FinishMode")
                    {
                        if (currentTarget != null)
                        {
                            finisherSlider.value = 25;
                            FinisherFullImage.SetActive(false);
                            StartCoroutine(EnterFinisherMode(usedSwordGrapple));
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
                if (!PillarFinisherUsed)
                {
                    currentTarget.transform.position = EnemyFinisherPlacement.position;
                    currentTarget.transform.rotation = EnemyFinisherPlacement.rotation;
                }
                if (Input.GetButtonDown("QuickFinish"))
                {
                    FailFinisherMode();
                    return;
                }
                ProcessPlayerInput();

                bool goodSoFar = false;
                foreach (FinisherAbstract RCombo in FinisherMoves)
                {
                    goodSoFar = RCombo.checkSoFar(RunicQue);
                    if (goodSoFar)
                    {
                        if (RCombo.check(RunicQue))
                        {
                            PrimaryAttackPopUp.SetActive(true);
                            if (PillarFinisherUsed)
                            {
                                if (PillarTutorial)
                                {
                                    PillarTutorial.PlayFinalHit();
                                }
                            }
                        }
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
                    foreach (FinisherAbstract f in FinisherMoves)
                    {
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
                    else
                    {
                        StartCoroutine(ExecuteFinisher(FinisherToPerform));
                    }
                }

                if (FinisherCount <= 0 && !ExecutingFinisher && !PillarFinisherUsed)
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

    //animate line
    private List<Direction> InputList = new List<Direction>();
    private ElementType CurrentFinisherElement;

    private void AddInput(Direction input)
    {
        switch (InputList.Count)
        {
            case 0:
                if (PillarFinisherUsed)
                {
                    if (PillarTutorial)
                    {
                        PillarTutorial.PlayFirstHit();
                    }
                }
                break;
            case 1:
                if (PillarFinisherUsed)
                {
                    if (PillarTutorial)
                    {
                        PillarTutorial.PlaySecondHit();
                    }
                }
                break;
            case 2:
                if (PillarFinisherUsed)
                {
                    if (PillarTutorial)
                    {
                        PillarTutorial.PlayThirdHit();
                    }
                }
                break;
            case 3:
                if (PillarFinisherUsed)
                {
                    if (PillarTutorial)
                    {
                        PillarTutorial.PlayThirdHit();
                    }
                }
                break;
        }

        //might be good to have the state of the element to help with the color
        //so it should be LeftToRightAnim(runic)
        InputList.Add(input);
        if(InputList.Count > 1)
        {
            switch (InputList[InputList.Count - 2])
            {
                case Direction.left:
                    switch(InputList[InputList.Count - 1])
                    {
                        case Direction.up:
                            FinisherLineAnimator.LeftToUpAnim(CurrentFinisherElement);
                            break;
                        case Direction.down:
                            FinisherLineAnimator.LeftToDownAnim(CurrentFinisherElement);
                            break;
                        case Direction.right:
                            FinisherLineAnimator.LeftToRightAnim(CurrentFinisherElement);
                            break;
                    }
                    break;
                case Direction.right:
                    switch (InputList[InputList.Count - 1])
                    {
                        case Direction.up:
                            FinisherLineAnimator.RightToUpAnim(CurrentFinisherElement);
                            break;
                        case Direction.left:
                            FinisherLineAnimator.RightToLeftAnim(CurrentFinisherElement);
                            break;
                        case Direction.down:
                            FinisherLineAnimator.RightToDownAnim(CurrentFinisherElement);
                            break;
                    }
                    break;
                case Direction.up:
                    switch (InputList[InputList.Count - 1])
                    {
                        case Direction.right:
                            FinisherLineAnimator.UpToRightAnim(CurrentFinisherElement);
                            break;
                        case Direction.left:
                            FinisherLineAnimator.UpToLeftAnim(CurrentFinisherElement);
                            break;
                    }
                    break;
                case Direction.down:
                    switch (InputList[InputList.Count - 1])
                    {
                        case Direction.right:
                            FinisherLineAnimator.DownToRightAnim(CurrentFinisherElement);
                            break;
                        case Direction.left:
                            FinisherLineAnimator.DownToLeftAnim(CurrentFinisherElement);
                            break;
                    }
                    break;
                default:
                    break;

            }
        }
        else
        {
            switch (input)
            {
                case Direction.left:
                    CurrentFinisherElement = ElementType.Fire;
                    break;
                case Direction.up:
                    CurrentFinisherElement = ElementType.Electricity;
                    break;
                case Direction.right:
                    CurrentFinisherElement = ElementType.Ice;
                    break;

            }
        }
    }

    [SerializeField] RuntimeAnimatorController enemyAnimatorController;
    RuntimeAnimatorController savedAnimController;
    Animator enemyAnimator;

    private void ProcessPlayerInput()
    {
        if (Input.GetButtonDown("UpButton"))
        {
            UIanim.Play("RunicUpCarve");
            CharAnim.Play("Up_Finisher");
            if(enemyAnimator)
                enemyAnimator.Play("Up_Finisher");
            RunicQue.Add(Direction.up);
            psc.PlayRunicStab(Direction.up);
            AddInput(Direction.up);
        }
        if (Input.GetButtonDown("RightButton"))
        {
            UIanim.Play("RunicRightCarve");
            CharAnim.Play("Right_Finisher");
            if (enemyAnimator)
                enemyAnimator.Play("Right_Finisher");
            RunicQue.Add(Direction.right);
            psc.PlayRunicStab(Direction.right);
            AddInput(Direction.right);
        }
        if (Input.GetButtonDown("DownButton"))
        {
            UIanim.Play("RunicDownCarve");
            CharAnim.Play("Down_Finisher");
            if (enemyAnimator)
                enemyAnimator.Play("Down_Finisher");
            RunicQue.Add(Direction.down);
            psc.PlayRunicStab(Direction.down);
            AddInput(Direction.down);
        }
        if (Input.GetButtonDown("LeftButton"))
        {
            UIanim.Play("RunicLeftCarve");
            CharAnim.Play("Left_Finisher");
            if (enemyAnimator)
                enemyAnimator.Play("Left_Finisher");
            RunicQue.Add(Direction.left);
            psc.PlayRunicStab(Direction.left);
            AddInput(Direction.left);
        }
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
    public IEnumerator EnterFinisherMode(bool usedSwordGrapple)
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
        if (!PillarFinisherUsed)
        {
            if (currentTarget.tag != "TargetDummy")
            {
                currentTarget.GetComponent<EnemyMovementController>().StopMovement();
                currentTarget.GetComponent<EnemyAI>().BeingFinished();

                if (currentTarget.GetComponent<EnemyTypeController>().MyEnemyType == EnemyType.Boss)
                {
                    savedAnimController = currentTarget.GetComponent<EnemyAI>().anim.runtimeAnimatorController;
                }

                if (enemyAnimator = currentTarget.GetComponent<EnemyAI>().anim)
                {
                    enemyAnimator.runtimeAnimatorController = enemyAnimatorController;
                    enemyAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    enemyAnimator.Play("Finisher_Start");
                }
                yield return null;
            }
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
            CharAnim.Play("Finisher_Start");
            yield return null;
        }
        else
        {
            CharAnim.Play("Finisher_Start");
            yield return null;
        }

        currentTarget.transform.position = EnemyFinisherPlacement.position;
        currentTarget.transform.rotation = EnemyFinisherPlacement.rotation;
        //currentTarget.transform.parent = EnemyFinisherPlacement;
        if (PillarFinisherUsed)
        {
            currentTarget.transform.position += Vector3.down;
            TutorialPopups.Instance.ShowTutorialPopup(PillarTutorial.FinisherUnlock);
        }

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
        if(!PillarFinisherUsed)
            RunicRinisherGuides.SetActive(true);

        //RunicSequence.RestartQue();
        RunicQue.Clear();
        InputList.Clear();
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
        CharAnim.Play("Finisher_End");
        if (PillarFinisherUsed)
        {
            PillarTutorial.PlayFinisherHit();
            FinisherToPerform.SetAsPiller();
        }
        else
        {
            if (enemyAnimator)
                enemyAnimator.Play("Finisher_Start");
        }
        yield return new WaitForSecondsRealtime(1f);
        FinisherToPerform.startfinisher(this);
        FinisherToPerform.RestoreAsPiller();

        yield return null; // do stuff to perform the finisher

        IncreaseGodModeMeter(PlayerDamageValues.Instance.ExecuteFinisherGMFill);
        StartCoroutine(LeavingFinisherMode(FinisherToPerform));
    }

    private bool didFail = false;
    public void FailFinisherMode()
    {
        PerformingFinisher = false;

        PrimaryAttackPopUp.SetActive(false);
        RunicRinisherGuides.SetActive(false);
        InFinisherIcons.SetActive(false);
        didFail = true;

        if (PillarFinisherUsed)
        {
            switch (PillarTutorial.FinisherUnlock)
            {
                case Finishers.Siphoning:
                    GetComponent<Siphoncut>().enabled = false;
                    break;
                case Finishers.FlameSword:
                    GetComponent<RunicFireSword>().enabled = false;
                    break;
                case Finishers.Flamethrower:
                    GetComponent<RunicFlamethrower>().enabled = false;
                    break;
                case Finishers.FlameAOE:
                    GetComponent<RunicFireCircle>().enabled = false;
                    break;
                case Finishers.FrostAOE:
                    GetComponent<RunicFrostCircle>().enabled = false;
                    break;
            }
        }

        StartCoroutine(LeavingFinisherMode());
    }

    [SerializeField] GameStatus gameStatus;
    [SerializeField] GameObject checkpoint;
    IEnumerator LeavingFinisherMode(FinisherAbstract finisherAbstractUsed = null)
    {
        UIanim.Play("idle");
        if (didFail)
        {
            CharAnim.Play("Finisher_End");
            yield return new WaitForSecondsRealtime(1f);
            CharAnim.Play("Idle");
        }
        Player.GetComponent<PlayerMovementController>().AllowMoving(); //MARK: barely noticable bug where if you move and do flamethrower finisher, you may move for 1 frame
        Player.GetComponent<PlayerMovementController>().AllowTurning();

        if (!PillarFinisherUsed)
        {
            if (currentTarget.tag != "TargetDummy")
                if (finisherAbstractUsed != null && (finisherAbstractUsed is Siphoncut)) //electricity is used for siphoning for now
                    currentTarget.GetComponent<EnemyAI>().KillEnemy(true);
                else
                {
                    currentTarget.GetComponent<EnemyAI>().KillEnemy();
                    enemyAnimator.Play("Death");
                    if(currentTarget.GetComponent<EnemyTypeController>().MyEnemyType == EnemyType.Boss)
                    {
                        currentTarget.GetComponent<EnemyAI>().anim.runtimeAnimatorController = savedAnimController;
                    }
                }
            else
                Destroy(currentTarget);

            if (currentTarget != null)
            {
                currentTarget.transform.parent = null;
                if (currentTarget.GetComponent<EnemyAI>() != null)
                {
                    currentTarget.GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.PrimaryAttacker);
                    currentTarget.GetComponent<EnemyMovementController>().HelpKnockback();
                }
            }
        }
        else if (!didFail) //TutorialPopupStuff
        {
            Invoke("HidePopup", 4f);
            gameStatus.CheckpointP = currentTarget.transform.position + currentTarget.transform.forward * 2;
            Invoke("SaveGame", 2f);
            IncreaseFinisherMeter(100);
            Destroy(currentTarget);
        }
        else
        {
            Invoke("HidePopup",0f);
        }
        didFail = false;

        inFinisherMode = false;
        ExecutingFinisher = false;
        PillarFinisherUsed = false;

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
    private float GrappleFinishRange = 15;
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
        GameObject testTarget = null;
        float lowestDotDistance = Mathf.Infinity;
        foreach (GameObject Enemy in Enemies)
        {
            if (!Enemy.GetComponent<EnemyAI>().enabled)
                continue;
            usedSwordGrapple = false;
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
                if(Vector3.Distance(Enemy.transform.position, transform.position) < 5)
                    testTarget = Enemy;
            }
        }
        if (!GameStatus.InCombat)
        {
            GameObject[] TargetDummies = GameObject.FindGameObjectsWithTag("TargetDummy");
            foreach (GameObject dummy in TargetDummies)
            {
                if (Vector3.Distance(dummy.transform.position, transform.position) < 4)
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
                if (Vector3.Distance(dummy.transform.position, transform.position) < 4)
                    testTarget = dummy;
            }
        }

        if (thisCurrentTarget != null)
        {
            FinisherIcon.SetActivated(true);
            if (thisCurrentDotTarget != null && Vector3.Distance(thisCurrentDotTarget.transform.position,this.transform.position) > 5)
            {
                FinisherIcon.transform.position = thisCurrentDotTarget.transform.position;
                usedSwordGrapple = true;
                if (thisCurrentDotTarget.tag == "TargetDummy")
                    usedSwordGrapple = false;
                return thisCurrentDotTarget;
            }
            FinisherIcon.transform.position = thisCurrentTarget.transform.position;
            return thisCurrentTarget;
        }
        else if (testTarget != null)
        {
            FinisherIcon.transform.position = testTarget.transform.position;
            return testTarget;
        }
        
        FinisherIcon.SetActivated(false);
        return null;
    }

}
