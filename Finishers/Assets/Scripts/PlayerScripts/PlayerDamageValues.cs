using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageValues : MonoBehaviour
{

    public static PlayerDamageValues Instance;

    //Player Attacks
    public float LightAttackDamage;

    //Runic Finisher Values

    //Flame Finisher
    public float FlamethrowerWaveDamage;
    public float FlameAOEDamage;

    //Frost Finisher
    public int FrostAOEFreezeCount;
    public float FrostAOEDamage;
    public float FrostAOEExplodeDamge;
    public float FrostTimeToMelt;

    //GodMode
    public float GodModeDamage;

    //finisher bar fill values
    public float NormalAttackFinMeterFill;
    public float ElementalSwordAttackFinMeterFill;
    public float SiphoningFinMeterFill;
    public float FlameThrowFinMeterFill;
    public float FlameAOEFinMeterFill;

    //GodMode increase values
    public float ExecuteFinisherGMFill;

    //PlayerHealthValues;
    public float SiphoningGainHealth;


    //EnemyValues
    public float NormalAttackDamage;
    public float JumpAttackDamage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

}