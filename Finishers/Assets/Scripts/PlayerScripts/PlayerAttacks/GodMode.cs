using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GodMode : MonoBehaviour {

    public Slider GodModeSlider;
    public GameObject GodModeText;
    public float GodModeTimer;
    private float GodModeCount;

	// Use this for initialization
	void Start () {
        GodModeCount = GodModeTimer;
	}

    private bool GodModePressed = false;
    public float GodModeDistance = 10;
    public PlayerMovementController pmc;
    public CameraMovementController cmc;
    public CameraFollow cf;
    public Animator CharAnim;
    public GameObject UpIcon;
    public GameObject RightIcon;
    public GameObject DownIcon;
    public GameObject LeftIcon;

    // Update is called once per frame
    void Update()
    {
        if (!GameStatus.GamePaused && !GameStatus.FinisherModeActive)
        {
            if (GodModeSlider.value >= 100)
            {
                GodModeText.SetActive(true);
                if (Input.GetButtonDown("GodMode"))
                {
                    GodModePressed = true;
                }
            }

            if (GodModePressed)
            {
                GodModeCount = 0;
                GodModePressed = false;
                GodModeText.SetActive(false);
                StartCoroutine(PerformGodMode());
            }
        }
    }

    IEnumerator PerformGodMode()
    {
        GetComponent<FinisherMode>().CanFinish = false;
        bool timeRanOut = false;
        bool firstEnemy = true;
        int kills = 0;
        List<GameObject> Enemies = GetEnemies();
        GameObject closestEnemy = null;
        //Enemies = BuildPathOfFoes(Enemies);
        if (Enemies.Count > 0)
        {
            pmc.PreventMoving();
            pmc.PreventTuring();
            GameStatus.FinisherModeActive = true;
            CharAnim.Play("Carve 1");
            closestEnemy = FindClosestEnemy(Enemies);

            //Go throug the nearby enemies
            while (kills < 5)
            {
                if (timeRanOut)
                    break;
                if (firstEnemy)
                {
                    //MARK: play an animation?? also make invulnerable

                    //moves camera
                    cmc.MoveToFinisherModeLocation(); //Mark make sure camera takes as long as the animation

                    while (cmc.GetIsMoving())
                    {
                        yield return null;
                    }
                    Time.timeScale = 0;
                    firstEnemy = false;
                    GetComponent<PlayerMovementController>().Aiming = false;
                }

                if (closestEnemy != null)
                {
                    CharAnim.Play("Idle");
                    //Go To enemy
                    //Prompt for button press
                    //if failed break out
                    //Perform Quick Finisher
                    StartCoroutine(MoveToEnemy(closestEnemy, .15f));
                    yield return new WaitForSecondsRealtime(.3f);

                    ChoseInput();

                    while (!CheckUserInput()) //returns true if the right button is pressed, so stop the while loop
                    {
                        if (GodModeCount >= GodModeTimer)// break out if they are taking too long
                        {
                            timeRanOut = true;
                            break;
                        }
                        GodModeCount += Time.unscaledDeltaTime;
                        yield return null;
                    }
                    HideInputs();
                    CharAnim.Play("FinisherExecution");
                    yield return new WaitForSecondsRealtime(1f);

                    GodModeSlider.value -= 34;
                    Destroy(closestEnemy);
                    GetComponent<FinisherMode>().IncreaseFinisherMeter(20);
                    kills++;
                    if (timeRanOut)
                        break;
                    yield return null;
                    Enemies = GetEnemies();
                    if (Enemies.Count <= 0)
                        break;
                    closestEnemy = FindClosestEnemy(Enemies);
                }
            }
        }

        Time.timeScale = 1;
        GetComponent<FinisherMode>().CanFinish = true;
        cmc.SwitchCombatLocation();
        pmc.AllowMoving();
        pmc.AllowTurning();
        GameStatus.FinisherModeActive = false;
        CharAnim.Play("Idle");
        yield return null;
    }

    int InputIndex = 0;
    public void ChoseInput()
    {
        UpIcon.transform.LookAt(cmc.transform);
        RightIcon.transform.LookAt(cmc.transform);
        DownIcon.transform.LookAt(cmc.transform);
        LeftIcon.transform.LookAt(cmc.transform);
        InputIndex = Random.Range(0, 4);
        switch (InputIndex)
        {
            case 0:
                UpIcon.SetActive(true);
                RightIcon.SetActive(false);
                DownIcon.SetActive(false);
                LeftIcon.SetActive(false);
                break;
            case 1:
                UpIcon.SetActive(false);
                RightIcon.SetActive(true);
                DownIcon.SetActive(false);
                LeftIcon.SetActive(false);
                break;
            case 2:
                UpIcon.SetActive(false);
                RightIcon.SetActive(false);
                DownIcon.SetActive(true);
                LeftIcon.SetActive(false);
                break;
            case 3:
                UpIcon.SetActive(false);
                RightIcon.SetActive(false);
                DownIcon.SetActive(false);
                LeftIcon.SetActive(true);
                break;

        }

    }
    public bool CheckUserInput()
    {
        if (Input.GetButtonDown("UpButton"))
        {
            if (InputIndex != 0)
            {
                GodModeCount = GodModeTimer;
                return false;
            }
            return true;
        }
        if (Input.GetButtonDown("RightButton"))
        {
            if (InputIndex != 1)
            {
                GodModeCount = GodModeTimer;
                return false;
            }
            return true;
        }
        if (Input.GetButtonDown("DownButton"))
        {
            if (InputIndex != 2)
            {
                GodModeCount = GodModeTimer;
                return false;
            }
            return true;
        }
        if (Input.GetButtonDown("LeftButton"))
        {
            if (InputIndex != 3)
            {
                GodModeCount = GodModeTimer;
                return false;
            }
            return true;
        }
        return false;
    }
    public void HideInputs()
    {
        UpIcon.SetActive(false);
        RightIcon.SetActive(false);
        DownIcon.SetActive(false);
        LeftIcon.SetActive(false);
    }

    public IEnumerator MoveToEnemy(GameObject enemy, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / timeToMove;
            var heading = transform.position - enemy.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            transform.position = Vector3.Lerp(currentPos, enemy.transform.position +  direction * 1.5f, t);//change direction to enemy.transform.forward to place in front of enemies
            Vector3 currentTargetPostition = new Vector3(enemy.transform.position.x, this.transform.position.y, enemy.transform.position.z);
            //PlayerRotWrapper.transform.LookAt(currentTargetPostition);
            cf.transform.LookAt(currentTargetPostition); //MARK: this is technically a bug, but the dynamic camera looks kinda good
            Quaternion rot = Quaternion.LookRotation(currentTargetPostition - transform.position);
            PlayerRotWrapper.transform.rotation = Quaternion.Slerp(PlayerRotWrapper.transform.rotation, rot, t);
            //cf.transform.rotation = Quaternion.Slerp(cf.transform.rotation, rot, 1);
            yield return null;
        }

        Vector3 targetPostition = new Vector3(enemy.transform.position.x, this.transform.position.y, enemy.transform.position.z);
        PlayerRotWrapper.transform.LookAt(targetPostition);
    }

    public Transform PlayerRotWrapper;

    public List<GameObject> GetEnemies()
    {
        List<GameObject> enemies = new List<GameObject>();

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject Enemy in Enemies)
        {
            if (Vector3.Distance(Enemy.transform.position, transform.position) < GodModeDistance && Enemy.GetComponent<NavMeshAgent>().isActiveAndEnabled)
            {
                enemies.Add(Enemy);
            }
        }
        GameObject[] TargetDummies = GameObject.FindGameObjectsWithTag("TargetDummy");
        foreach (GameObject dummy in TargetDummies)
        {
            if (Vector3.Distance(dummy.transform.position, transform.position) < GodModeDistance)
            {
                enemies.Add(dummy);
            }
        }
        return enemies;
    }

    public GameObject FindClosestEnemy(List<GameObject> enemies)
    {
        GameObject closest = enemies[0];
        foreach (GameObject enemy in enemies) {
            if (Vector3.Distance(enemy.transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
            {
                closest = enemy;
            }   
        }
        return closest;
    }

}
