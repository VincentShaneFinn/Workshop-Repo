using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPillar : MonoBehaviour {

    private GameObject player;
    private FinisherMode fm;
    private bool inRange = false;
    public Finishers FinisherUnlock;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        fm = player.GetComponent<FinisherMode>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            inRange = true;
            fm.PillarFinisherNearby = true;
            fm.PillarTarget = gameObject;
        }
        else if (inRange)
        {
            inRange = false;
            fm.PillarFinisherNearby = false;
            fm.FinisherIcon.SetActivated(false);
        }
	}
}
