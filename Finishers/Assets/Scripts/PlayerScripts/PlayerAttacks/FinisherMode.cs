using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherMode : MonoBehaviour {

    public float FinisherTime;
    private float FinisherCount;

    void Start()
    {
        FinisherCount = FinisherTime;
    }

	// Update is called once per frame
	void Update () {
        if (FinisherCount >= FinisherTime)
        {
            Time.timeScale = 1;
            if (Input.GetKeyDown(KeyCode.F))
            {
                FinisherCount = 0;
                Time.timeScale = .1f;
            }
        }
        else
        {
            FinisherCount += Time.deltaTime;
        }
	}
}
