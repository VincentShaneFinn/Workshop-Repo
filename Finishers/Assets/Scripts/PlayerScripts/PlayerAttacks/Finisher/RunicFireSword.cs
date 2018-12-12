using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicFireSword : FinisherAbstract {
    // Use this for initialization
    public float SwordTimer;
    private float SwordCount;
    public PlayerSwordHit sword;
    public GameObject FlameSword;
    public GameObject Flames;
    private GameObject currentflame;
    private bool LightSword = false;

    // Use this for initialization
    void OnEnable()
    {
        GetComponent<FinisherMode>().AddFinisherMove(this);
        SwordCount = SwordTimer;
    }

    void OnDisable()
    {
        GetComponent<FinisherMode>().RemoveFinisherMove(this);
    }

    void Update()
    {
        if (!GameStatus.GamePaused && !GameStatus.FinisherModeActive)
        {
            if (LightSword)
            {
                LightSword = false;
                SwordCount = 0;
                currentflame = Instantiate(Flames, sword.gameObject.transform.position, sword.gameObject.transform.rotation);
                currentflame.transform.parent = sword.swordEdge.transform;
                Destroy(currentflame, .4f);
            }

            if (SwordCount < SwordTimer)
            {
                FlameSword.SetActive(true);
                sword.SetFireSkin();
                sword.SetSwordDamage(PlayerDamageValues.Instance.ElementalSwordDamage);
            }
            else
            {
                FlameSword.SetActive(false);
                sword.RestoreSwordDamage();
                sword.RestoreSwordSkin();
            }

            SwordCount += Time.deltaTime;
        }
    }

    public override void startfinisher(FinisherMode f) {
        //Do stuff here
        LightSword = true;
        f.CharAnim.Play("Idle");
        print("Commit Runit Finisher");
    }
}
