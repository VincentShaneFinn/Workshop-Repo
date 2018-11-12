using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordHit : MonoBehaviour {

    public FinisherMode finisherObject;
    public int swordDamage = 1;
    private int currentSwordDamage;
    public SkinnedMeshRenderer swordEdge;
    public Material[] OriginalSwordMaterials;
    public Material[] FireMats;

    void Start()
    {
        currentSwordDamage = swordDamage;
    }

    public void SetSwordDamage(int d)
    {
        currentSwordDamage = d;
    }
    public void RestoreSwordDamage()
    {
        currentSwordDamage = swordDamage;
    }
    public void SetFireSkin()
    {
        swordEdge.materials = FireMats;
    }
    public void RestoreSwordSkin()
    {
        swordEdge.materials = OriginalSwordMaterials;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Enemy"))
        {
            finisherObject.IncreaseFinisherMeter();
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage(currentSwordDamage);
            }
        }
        else if (col.gameObject.tag.Equals("TargetDummy"))
        {
            finisherObject.IncreaseFinisherMeter();
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage(currentSwordDamage);
            }
        }

    }
}
