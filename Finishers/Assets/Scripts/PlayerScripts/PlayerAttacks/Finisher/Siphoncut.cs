using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siphoncut : FinisherAbstract {
    public GameObject TopHalf;
    public GameObject BottomHalf;
    public GameObject SlicedLimb;
    public Transform SlicedLimbFirePoint;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public override void startfinisher(FinisherMode f)
    {
        GameObject part1 = Instantiate(TopHalf, new Vector3(f.currentTarget.transform.position.x, 1f, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
        GameObject part2 = Instantiate(BottomHalf, new Vector3(f.currentTarget.transform.position.x, 0f, f.currentTarget.transform.position.z), f.currentTarget.transform.rotation);
        try { Instantiate(SlicedLimb, SlicedLimbFirePoint); } catch { }
        GetComponent<PlayerHealthController>().PlayerHealed(20);
        f.anim.Play("SlashL");
    }
}
