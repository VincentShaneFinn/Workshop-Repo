using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour {

    public static bool GamePaused;
    public static bool FinisherModeActive;

    void Start()
    {
        GamePaused = false;
        FinisherModeActive = false;
    }
}
