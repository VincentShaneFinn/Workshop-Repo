using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour {

    public static bool GamePaused;
    public static bool FinisherModeActive;
    public static bool InCombat;

    void Start()
    {
        GamePaused = false;
        FinisherModeActive = false;
        InCombat = false;
        Time.timeScale = 1;
    }
}
