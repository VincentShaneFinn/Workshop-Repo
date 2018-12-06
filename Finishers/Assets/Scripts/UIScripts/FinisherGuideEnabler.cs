using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherGuideEnabler : MonoBehaviour {

    [SerializeField] GameObject siphonTut;
    [SerializeField] GameObject fireSwordTut;
    [SerializeField] GameObject flamethrowerTut;
    [SerializeField] GameObject fireCircleTut;
    [SerializeField] GameObject frostCircleTut;

    GameObject player;
    Siphoncut siphoncut;
    RunicFireSword fireSword;
    RunicFlamethrower flamethrower;
    RunicFireCircle fireCircle;
    RunicFrostCircle frostCircle;
	
	// Update is called once per frame
	void OnEnable () {
        player = GameObject.FindGameObjectWithTag("Player");
        siphoncut = player.GetComponent<Siphoncut>();
        fireSword = player.GetComponent<RunicFireSword>();
        flamethrower = player.GetComponent<RunicFlamethrower>();
        fireCircle = player.GetComponent<RunicFireCircle>();
        frostCircle = player.GetComponent<RunicFrostCircle>();

        siphonTut.SetActive(false);
        fireSwordTut.SetActive(false);
        flamethrowerTut.SetActive(false);
        fireCircleTut.SetActive(false);
        frostCircleTut.SetActive(false);

        if (siphoncut.enabled)
        {
            siphonTut.SetActive(true);
        }
        if (fireSword.enabled)
        {
            fireSwordTut.SetActive(true);
        }
        if (flamethrower.enabled)
        {
            flamethrowerTut.SetActive(true);
        }
        if (fireCircle.enabled)
        {
            fireCircleTut.SetActive(true);
        }
        if (frostCircle.enabled)
        {
            frostCircleTut.SetActive(true);
        }

	}
}
