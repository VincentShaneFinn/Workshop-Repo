using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiphonHolsterController : MonoBehaviour {

    private List<GameObject> Swords;
    private GameObject CurrentSword;
    public GameObject ThrowableSword;
    public int swordLimit = 4;
    public Transform RotatingRing;
    public CameraMovementController CMC;
    private bool buttonPressed = false;
    private bool cancelAction = false;

    // Use this for initialization
    void Start()
    {
        Swords = new List<GameObject>();
    }
    
    // Update is called once per frame
	void Update () {
        if (!GameStatus.GamePaused)
        {
            if(buttonPressed && GameStatus.FinisherModeActive)
            {
                cancelAction = true;
                CurrentSword.GetComponent<ThrowLimb>().DestroyLine();
                return;
            }
            if(CurrentSword == null)
            {
                return;
            }
            if (Input.GetButtonDown("SpecialAttack"))
            {
                CMC.MoveToAimingLocation();
                buttonPressed = true;
            }
            if (Input.GetButton("SpecialAttack"))
            {
                if (CurrentSword != null && buttonPressed)
                    CurrentSword.GetComponent<ThrowLimb>().ButtonHeld();
            }
            if (Input.GetButtonUp("SpecialAttack"))
            {
                buttonPressed = false;
                if (cancelAction)
                {
                    CurrentSword.GetComponent<ThrowLimb>().DestroyLine();
                    cancelAction = false;
                    return;
                }
                CMC.ReturnFromAimingLocation();
                if (CurrentSword != null)
                    CurrentSword.GetComponent<ThrowLimb>().ButtonReleased();
                if (Swords.Count > 0)
                {
                    CurrentSword = Swords[0];
                    Swords.RemoveAt(0);
                    CurrentSword.transform.parent = transform;
                    CurrentSword.transform.localPosition = Vector3.zero;
                    CurrentSword.transform.localRotation = Quaternion.identity;

                }
                else
                {
                    CurrentSword = null;
                }
            }
            int i = 0;
            foreach (GameObject sword in Swords)
            {
                sword.transform.parent = RotatingRing;
                switch (i)
                {
                    case 0:
                        sword.transform.localPosition = Vector3.up * .2f;
                        break;
                    case 1:
                        sword.transform.localPosition = Vector3.down * .2f;
                        break;
                    case 2:
                        sword.transform.localPosition = Vector3.right * .2f;
                        break;
                    case 3:
                        sword.transform.localPosition = Vector3.left * .2f;
                        break;
                }
                sword.transform.localRotation = Quaternion.identity;
                i++;
            }
            RotatingRing.Rotate(new Vector3(0, 0, 1));
        }
    }

    public void AddSword()
    {
        if (CurrentSword == null)
            CurrentSword = Instantiate(ThrowableSword, gameObject.transform);
        else if(Swords.Count < swordLimit)
        {
            Swords.Add(Instantiate(ThrowableSword, gameObject.transform));
        }
    }
}
