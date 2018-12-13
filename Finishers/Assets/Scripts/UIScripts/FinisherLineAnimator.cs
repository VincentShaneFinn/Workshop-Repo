using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType { Fire, Ice, Electricity } //using electricity for siphoning for now

// todo refactor to behave like TutorialPopups
public class FinisherLineAnimator : MonoBehaviour {

    static FinisherLineAnimator lineAnimator;

    public GameObject LeftToRightRed;
    public GameObject LeftToUpRed;
    public GameObject LeftToDownBlue;
    public GameObject LeftToDownRed;
    public GameObject RightToLeftRed;
    public GameObject RightToLeftGreen;
    public GameObject RightToUpBlue;
    public GameObject RightToDownRed;
    public GameObject UpToRightRed;
    public GameObject UpToRightGreen;
    public GameObject UpToLeftBlue;
    public GameObject DownToLeftRed;
    public GameObject DownToRightBlue;
    public GameObject DownToRightRed;

    private void Awake()
    {
        if (lineAnimator == null) { lineAnimator = this; } // Be the one
    }

    private void OnDisable()
    {
        LeftToRightRed.SetActive(false);
        LeftToUpRed.SetActive(false);
        LeftToDownBlue.SetActive(false);
        RightToLeftRed.SetActive(false);
        RightToUpBlue.SetActive(false);
        RightToDownRed.SetActive(false);
        LeftToDownRed.SetActive(false);
        RightToLeftGreen.SetActive(false);
        UpToRightRed.SetActive(false);
        UpToRightGreen.SetActive(false);
        UpToLeftBlue.SetActive(false);
        DownToLeftRed.SetActive(false);
        DownToRightBlue.SetActive(false);
        DownToRightRed.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void LeftToRightAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.LeftToRightRed.SetActive(true);
                break;
        }
    }

    public static void LeftToUpAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.LeftToUpRed.SetActive(true);
                break;
        }
    }

    public static void LeftToDownAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.LeftToDownRed.SetActive(true);
                break;
            case ElementType.Ice:
                lineAnimator.LeftToDownBlue.SetActive(true);
                break;
        }
    }

    public static void RightToLeftAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.RightToLeftRed.SetActive(true);
                break;
            case ElementType.Electricity:
                lineAnimator.RightToLeftGreen.SetActive(true);
                break;
        }
    }

    public static void RightToUpAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Ice:
                lineAnimator.RightToUpBlue.SetActive(true);
                break;
        }
    }

    public static void RightToDownAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.RightToDownRed.SetActive(true);
                break;
        }
    }

    public static void UpToRightAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.UpToRightRed.SetActive(true);
                break;
            case ElementType.Electricity:
                lineAnimator.UpToRightGreen.SetActive(true);
                break;
        }
    }

    public static void UpToLeftAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Ice:
                lineAnimator.UpToLeftBlue.SetActive(true);
                break;
        }
    }

    public static void DownToLeftAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Fire:
                lineAnimator.DownToLeftRed.SetActive(true);
                break;
        }
    }

    public static void DownToRightAnim(ElementType et)
    {
        switch (et)
        {
            case ElementType.Ice:
                lineAnimator.DownToRightBlue.SetActive(true);
                break;
            case ElementType.Fire:
                lineAnimator.DownToRightRed.SetActive(true);
                break;
        }
    }
}
