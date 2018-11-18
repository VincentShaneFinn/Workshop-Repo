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
    public PlayerSwordHit sword;
    public GameObject Flames;
    private GameObject currentflame;

	// Use this for initialization
	void Start () {
        GodModeCount = GodModeTimer;
	}

    //// Update is called once per frame
    //void Update () {
    //       if (!GameStatus.GamePaused && !GameStatus.FinisherModeActive)
    //       {
    //           if (GodModeSlider.value >= 100)
    //           {
    //               GodModeText.SetActive(true);
    //               if (Input.GetButtonDown("GodMode"))
    //               {
    //                   GetComponent<FinisherMode>().IncreaseFinisherMeter(100);
    //                   GodModeCount = 0;
    //                   GodModeSlider.value = 0;
    //                   GodModeText.SetActive(false);
    //                   currentflame = Instantiate(Flames, sword.gameObject.transform.position, sword.gameObject.transform.rotation);
    //                   currentflame.transform.parent = sword.swordEdge.transform;
    //                   Destroy(currentflame, .2f);
    //               }
    //           }

    //           if (GodModeCount < GodModeTimer)
    //           {
    //               sword.SetFireSkin();
    //               sword.SetSwordDamage(PlayerDamageValues.Instance.GodModeDamage);
    //           }
    //           else
    //           {
    //               sword.RestoreSwordDamage();
    //               sword.RestoreSwordSkin();
    //           }

    //           GodModeCount += Time.deltaTime;
    //       }
    //}

    private bool GodModePressed = false;
    public float GodModeDistance = 10;
    public PlayerMovementController pmc;
    public CameraMovementController cmc;
    public CameraFollow cf;
    public Animator CharAnim;

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
                GodModeSlider.value = 0;
                GodModeText.SetActive(false);
                GetComponent<FinisherMode>().IncreaseFinisherMeter(100);
                StartCoroutine(PerformGodMode());
            }
        }
    }

    IEnumerator PerformGodMode()
    {
        GetComponent<FinisherMode>().CanFinish = false;
        List<GameObject> Enemies = GetEnemies(5);
        bool timeRanOut = false;
        bool firstEnemy = true;
        pmc.PreventMoving();
        pmc.PreventTuring();
        GameStatus.FinisherModeActive = true;
        CharAnim.Play("FinisherStart");

        //Go throug the nearby enemies
        foreach(GameObject Enemy in Enemies)
        {
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

            if (Enemy != null)
            {
                //Go To enemy
                //Prompt for button press
                //if failed break out
                //Perform Quick Finisher
                transform.position = Enemy.transform.position + Enemy.transform.forward * 1.5f;
                Vector3 targetPostition = new Vector3(Enemy.transform.position.x,
                this.transform.position.y,
                Enemy.transform.position.z);
                PlayerRotWrapper.transform.LookAt(targetPostition);
                cf.transform.LookAt(targetPostition); //MARK: this is technically a bug, but the dynamic camera looks kinda good
                while (!Input.GetKeyDown(KeyCode.Tab))
                {
                    if (GodModeCount >= GodModeTimer)// break out if they are taking too long
                    {
                        timeRanOut = true;
                        break;
                    }
                    GodModeCount += Time.unscaledDeltaTime;
                    yield return null;
                }
                if (timeRanOut)
                    break;
                CharAnim.Play("FinisherExecution");
                yield return new WaitForSecondsRealtime(1f);
                CharAnim.Play("Idle");
                Destroy(Enemy);
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

    public Transform PlayerRotWrapper;

    public List<GameObject> GetEnemies(int n)
    {
        List<GameObject> enemies = new List<GameObject>();

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject Enemy in Enemies)
        {
            if (Vector3.Distance(Enemy.transform.position, transform.position) < GodModeDistance && Enemy.GetComponent<NavMeshAgent>().isActiveAndEnabled)
            {
                enemies.Add(Enemy);
                if (enemies.Count >= n)
                    return enemies;
            }
        }
        GameObject[] TargetDummies = GameObject.FindGameObjectsWithTag("TargetDummy");
        foreach (GameObject dummy in TargetDummies)
        {
            if (Vector3.Distance(dummy.transform.position, transform.position) < GodModeDistance)
            {
                enemies.Add(dummy);
                if (enemies.Count >= n)
                    return enemies;
            }
        }
        return enemies;
    }
}
