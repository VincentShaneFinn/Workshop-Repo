using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType { PC, PS4, Xbox }
public class GameStatus : MonoBehaviour {

    public static bool GamePaused;
    public static bool FinisherModeActive;
    public static bool InCombat;
    public static ControlType CurrentControlType;

    void Start()
    {
        GamePaused = false;
        FinisherModeActive = false;
        InCombat = false;
        Time.timeScale = 1;
    }
}
