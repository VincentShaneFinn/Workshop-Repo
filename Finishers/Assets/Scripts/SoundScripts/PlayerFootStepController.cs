using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MARK: Being used for sword attacks as well right now
public class PlayerFootStepController : MonoBehaviour {

    public bool step = false;
    private bool didStep = false;
    private bool foot1Last = false;
    public AudioSource foot1;
    public AudioSource foot2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(step == true)
        {
            step = false;
            if (!didStep)
            {
                if (!foot1Last)
                {
                    foot1.Play();
                    foot1Last = true;
                }
                else
                {
                    foot2.Play();
                    foot1Last = false;
                }
                didStep = true;
            }
        }
        else
        {
            didStep = false;
        }
	}
}
