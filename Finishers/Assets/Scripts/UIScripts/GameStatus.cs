﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType { PC, PS4, Xbox }
public class GameStatus : MonoBehaviour {

    public static bool GamePaused;
    public static bool FinisherModeActive;
    public static bool InCombat;
    public static ControlType CurrentControlType;
    public static bool LoadGameBool = false;

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


    public void SaveGame()
    {
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
            GetComponent<UIManager>().RestartGame();
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
        Invoke("LoadGame", 3);
    }
}
