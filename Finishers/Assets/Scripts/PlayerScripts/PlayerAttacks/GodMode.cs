using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodMode : MonoBehaviour {

    public Slider GodModeSlider;
    public GameObject GodModeText;
    public float GodModeTimer;
    private float GodModeCount;
    public PlayerSwordHit sword;
    public GameObject Flames;
    private GameObject currentflame;

	// Use this for initialization
	void Start () {
        GodModeCount = GodModeTimer;
	}
	
	// Update is called once per frame
	void Update () {
        if (GodModeSlider.value >= 100)
        {
            GodModeText.SetActive(true);
            if (Input.GetButtonDown("GodMode"))
            {
                GodModeCount = 0;
                GodModeSlider.value = 0;
                GodModeText.SetActive(false);
                currentflame = Instantiate(Flames,sword.gameObject.transform.position, sword.gameObject.transform.rotation);
                currentflame.transform.parent = sword.swordEdge.transform;
                Destroy(currentflame, .2f);
            }
        }

        if (GodModeCount < GodModeTimer)
        {
            sword.SetFireSkin();
            sword.SetSwordDamage(2);
        }
        else
        {
            sword.RestoreSwordDamage();
            sword.RestoreSwordSkin();
        }

        GodModeCount += Time.deltaTime;
	}
}
