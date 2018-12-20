using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ControlType { PC, PS4, Xbox }
public class GameStatus : MonoBehaviour {

    public static bool GamePaused;
    public static bool FinisherModeActive;
    public static bool InCombat;
    public static ControlType CurrentControlType;
    public static bool LoadGameBool = false;
    public static int GroupsDefeated = 0;

    void Start()
    {
        Debug.Log(gameObject);
        GamePaused = false;
        FinisherModeActive = false;
        InCombat = false;
        Time.timeScale = 1;
        fm = GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>();
        if (LoadGameBool)
        {
            Invoke("LoadGame", .05f);
        }
        else
        {
            CheckpointP = transform.parent.position;
            Invoke("SaveGame", .05f);
        }
    }

    public GameObject Key1;
    public GameObject Key2;
    public GameObject Key3;
    public GameObject Door1;
    public GameObject Door2;
    public GameObject KeyText;
    private bool openedDoors = false;
    [SerializeField] bool usingKeys = false;

    private void Update()
    {
        Key1.SetActive(false);
        Key2.SetActive(false);
        Key3.SetActive(false);
        KeyText.SetActive(false);
        if (Door1 == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Siphoncut>().enabled = true;
            player.GetComponent<RunicFlamethrower>().enabled = true;
            player.GetComponent<RunicFireCircle>().enabled = true;
            player.GetComponent<RunicFireSword>().enabled = true;
            player.GetComponent<RunicFrostCircle>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Pillars[0].transform.position + Pillars[0].transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Pillars[1].transform.position + Pillars[1].transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Pillars[2].transform.position + Pillars[2].transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Pillars[3].transform.position + Pillars[3].transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Pillars[4].transform.position + Pillars[4].transform.forward;
        }
        if (!openedDoors && Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, Door1.transform.position) < 10)
        {
            openedDoors = true;
            StartCoroutine(OpenDoors());
        }
    }

    IEnumerator OpenDoors()
    {
        float timeToOpen = 5;
        float count = 0;
        while (count <= timeToOpen)
        {
            count += Time.deltaTime;
            Door1.transform.Translate(Vector3.right * 1f * Time.deltaTime);
            Door2.transform.Translate(Vector3.left * 1f * Time.deltaTime);
            yield return null;
        }


    }

    public List<GameObject> Groups;
    public List<GameObject> Pillars;
    public Transform playerT;
    public Slider HealthSlider;
    public Slider FinisherSlider;
    public Vector3 CheckpointP;
    private FinisherMode fm;

    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        int i = 0;
        foreach (GameObject targetGameObject in Groups)
        {
            if (!targetGameObject.activeSelf)
            {
                save.DeadGroups.Add(i);
            }
            i++;
        }
        i = 0;
        foreach (GameObject targetGameObject in Pillars)
        {
            if (targetGameObject == null || !targetGameObject.activeSelf)
            {
                save.FinishedPillars.Add(i);
            }
            i++;
        }

        save.healthMeter = HealthSlider.value ;
        save.finisherMeter = FinisherSlider.value;

        save.playerX = CheckpointP.x;
        save.playerY = CheckpointP.y;
        save.playerZ = CheckpointP.z;

        return save;
    }

    public Text saveGamePopup;
    private bool ignoreFirst = false;
    private void DelaySaveGameTextRemove()
    {
        saveGamePopup.text = "";
    }

    public void SaveGame()
    {
        if (!ignoreFirst)
            ignoreFirst = true;
        else
        {
            saveGamePopup.text = "Saved Game";
            Invoke("DelaySaveGameTextRemove", 3);
        }
        // 1
        Save save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        // 3 puts into a default state but we dont want that
        //hits = 0;
        //shots = 0;
        //shotsText.text = "Shots: " + shots;
        //hitsText.text = "Hits: " + hits;

        //ClearRobots();
        //ClearBullets();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (!LoadGameBool)
        {
            LoadGameBool = true;
            int index = SceneManager.GetActiveScene().buildIndex;//reload scene
            SceneManager.LoadScene(index);
            return;
        }
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 3
            foreach (int index in save.DeadGroups)
            {
                Groups[index].SetActive(false);
            }

            List<FinisherAbstract> FinishersUnlocked = new List<FinisherAbstract>();
            foreach (int index in save.FinishedPillars)
            {
                TutorialPillar pillarTutorial = Pillars[index].GetComponent<TutorialPillar>();
                switch (pillarTutorial.FinisherUnlock)
                {
                    case Finishers.Siphoning:
                        fm.GetComponent<Siphoncut>().enabled = true;
                        break;
                    case Finishers.FlameSword:
                        fm.GetComponent<RunicFireSword>().enabled = true;
                        break;
                    case Finishers.Flamethrower:
                        fm.GetComponent<RunicFlamethrower>().enabled = true;
                        break;
                    case Finishers.FlameAOE:
                        fm.GetComponent<RunicFireCircle>().enabled = true;
                        break;
                    case Finishers.FrostAOE:
                        fm.GetComponent<RunicFrostCircle>().enabled = true;
                        break;
                }
                Pillars[index].SetActive(false);
            }

            // 4
            HealthSlider.value = save.healthMeter;
            FinisherSlider.value = save.finisherMeter;
            GroupsDefeated = save.DeadGroups.Count;

            playerT.position =new Vector3(save.playerX, save.playerY, save.playerZ);
            playerT.GetComponent<Rigidbody>().velocity = Vector3.zero;

            Debug.Log("Game Loaded");

            GetComponent<UIManager>().ResumeGame();
        }
        else
        {
            Debug.Log("No game saved!");
        }
        LoadGameBool = false;
    }

    public void PlayerDied()
    {
        Invoke("LoadGame", 1.5f);
    }
}
