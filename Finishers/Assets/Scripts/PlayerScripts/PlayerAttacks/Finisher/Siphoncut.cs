using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siphoncut : FinisherAbstract {
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public Transform SlicedLimbFirePoint;
    public SiphonHolsterController shc;
	// Use this for initialization
	void OnEnable () {
        GetComponent<FinisherMode>().AddFinisherMove(this);
    }

    void OnDisable()
    {
        GetComponent<FinisherMode>().RemoveFinisherMove(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void startfinisher(FinisherMode f)
    {
        if (!isPillar)
        {
            GameObject part1 = Instantiate(TopHalf, new Vector3(f.currentTarget.transform.position.x, transform.position.y, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
            GameObject part2 = Instantiate(BottomHalf, new Vector3(f.currentTarget.transform.position.x, transform.position.y - 1f, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
        }

        shc.AddSword();
        GetComponent<PlayerHealthController>().PlayerHealed(20);
        f.CharAnim.Play("Attack 1");
        GetComponent<PlayerSoundController>().PlaySiphonCut();
    }
}
