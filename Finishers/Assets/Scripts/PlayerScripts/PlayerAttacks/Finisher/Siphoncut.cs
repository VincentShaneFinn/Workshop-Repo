using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siphoncut : FinisherAbstract {
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public Transform SlicedLimbFirePoint;
    public SiphonHolsterController shc;
	// Use this for initialization
	void Start () {
        GetComponent<FinisherMode>().AddFinisherMove(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void startfinisher(FinisherMode f)
    {
        GameObject part1 = Instantiate(TopHalf, new Vector3(f.currentTarget.transform.position.x, 1.5f, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
        GameObject part2 = Instantiate(BottomHalf, new Vector3(f.currentTarget.transform.position.x, 0.5f, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
        shc.AddSword();
        GetComponent<PlayerHealthController>().PlayerHealed(20);
        f.CharAnim.Play("Attack 1");
        GetComponent<PlayerSoundController>().PlaySiphonCut();
    }
}
