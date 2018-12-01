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
        GamePaused = false;
        FinisherModeActive = false;
        InCombat = false;
        Time.timeScale = 1;
        if (LoadGameBool)
        {
            LoadGame();
        }
        else
        {
            CheckpointP = transform.parent.position;
            SaveGame();
        }
    }

    public GameObject Key1;
    public GameObject Key2;
    public GameObject Key3;
    public GameObject Door1;
    public GameObject Door2;
    public GameObject KeyText;
    private bool openedDoors = false;

    private void Update()
    {
        if (Door1 == null)
        {
            Key1.SetActive(false);
            Key2.SetActive(false);
            Key3.SetActive(false);
            KeyText.SetActive(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GroupsDefeated = 8;
        }
        if(GroupsDefeated >= 3)
        {
            Key1.SetActive(true);
        }
        if(GroupsDefeated >= 6)
        {
            Key2.SetActive(true);
        }
        if(GroupsDefeated >= 9)
        {
            Key3.SetActive(true);
            if (!openedDoors && Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, Door1.transform.position) < 20)
            {
                openedDoors = true;
                StartCoroutine(OpenDoors());
            }
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
    public Transform playerT;
    public Slider HealthSlider;
    public Slider FinisherSlider;
    public Vector3 CheckpointP;

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

            // 4
            HealthSlider.value = save.healthMeter;
            FinisherSlider.value = save.finisherMeter;
            GroupsDefeated = save.DeadGroups.Count;

            playerT.position =new Vector3(save.playerX, save.playerY, save.playerZ);

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
