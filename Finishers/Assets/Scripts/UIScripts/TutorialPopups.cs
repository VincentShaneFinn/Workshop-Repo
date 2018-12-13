using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Finishers { Siphoning, FlameSword, Flamethrower, FlameAOE, FrostAOE}

public class TutorialPopups : MonoBehaviour {

    public static TutorialPopups Instance { get; private set; }

    [SerializeField] GameObject SiphoningPopup;
    [SerializeField] GameObject FlameSwordPopup;
    [SerializeField] GameObject FlamethrowerPopup;
    [SerializeField] GameObject FlameAOEPopup;
    [SerializeField] GameObject FrostAOEPopup;

    GameObject activePopup;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } // Be the one
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HideTutorialPopup();
            ShowTutorialPopup(Finishers.Siphoning);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HideTutorialPopup();
            ShowTutorialPopup(Finishers.FlameSword);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HideTutorialPopup();
            ShowTutorialPopup(Finishers.Flamethrower);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HideTutorialPopup();
            ShowTutorialPopup(Finishers.FlameAOE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HideTutorialPopup();
            ShowTutorialPopup(Finishers.FrostAOE);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            HideTutorialPopup();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            HideTutorialPopup();
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            HideTutorialPopup();
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            HideTutorialPopup();
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            HideTutorialPopup();
        }
    }

    public void ShowTutorialPopup(Finishers finisher)
    {
        switch (finisher)
        {
            case Finishers.Siphoning:
                activePopup = SiphoningPopup;
                break;
            case Finishers.FlameSword:
                activePopup = FlameSwordPopup;
                break;
            case Finishers.Flamethrower:
                activePopup = FlamethrowerPopup;
                break;
            case Finishers.FlameAOE:
                activePopup = FlameAOEPopup;
                break;
            case Finishers.FrostAOE:
                activePopup = FrostAOEPopup;
                break;
        }
        activePopup.SetActive(true);
    }

    public void HideTutorialPopup()
    {
        if(activePopup != null)
            activePopup.SetActive(false);
    }
}
