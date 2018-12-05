using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordHit : MonoBehaviour {

    public FinisherMode finisherObject;
    public float swordDamage = 1;
    private float currentSwordDamage;
    public SkinnedMeshRenderer swordEdge;
    public Material[] OriginalSwordMaterials;
    public Material[] FireMats;
    public AttackType CurrentAttackType;
    private bool isFinisher = false;

    void Start()
    {
        swordDamage = PlayerDamageValues.Instance.LightAttackDamage;
        currentSwordDamage = swordDamage;
        CurrentAttackType = AttackType.NormalBlade;
    }

    public void SetSwordDamage(float d)
    {
        currentSwordDamage = d;
    }
    public void RestoreSwordDamage()
    {
        currentSwordDamage = swordDamage;
        isFinisher = false;
    }
    public void SetFireSkin()
    {
        swordEdge.materials = FireMats;
        isFinisher = true;
        CurrentAttackType = AttackType.Fire;
    }
    public void RestoreSwordSkin()
    {
        swordEdge.materials = OriginalSwordMaterials;
        CurrentAttackType = AttackType.NormalBlade;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Enemy"))
        {
            if (CurrentAttackType == AttackType.NormalBlade)
            {
                finisherObject.IncreaseFinisherMeter(PlayerDamageValues.Instance.NormalAttackFinMeterFill);
            }
            else
            {
                finisherObject.IncreaseFinisherMeter(PlayerDamageValues.Instance.ElementalSwordAttackFinMeterFill);
            }
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage(currentSwordDamage, CurrentAttackType);
                Debug.Log(gameObject);
            }
        }
        else if (col.gameObject.tag.Equals("TargetDummy"))
        {
            finisherObject.IncreaseFinisherMeter(PlayerDamageValues.Instance.NormalAttackFinMeterFill);
            Enemyhp e = null;
            if ((e = col.GetComponent<Enemyhp>()) != null)
            {
                e.damage(currentSwordDamage, CurrentAttackType);
            }
        }

    }
}
