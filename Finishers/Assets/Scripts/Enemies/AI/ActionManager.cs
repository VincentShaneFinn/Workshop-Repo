﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class lets AI take an arc angle and give it back

public class ActionManager {

    public int MaxAttackActions = 3;// compare to the sum of all tracked attack actions
    public int MaxNormalAttacks = 2;
    public int CurrentNormalAttacks = 0;
    public int MaxSpecial1Attacks = 2;
    public int CurrentSpecial1Attacks = 0;

    public ActionManager() { }

    public bool TryNormalAttack()
    {
        if (CanPerfromAttackAction() && CanPerformNormal())
        {
            CurrentNormalAttacks++;
            return true;
        }
        return false;
    }

    public void NormalAttackCompleted()
    {
        CurrentNormalAttacks--;
        if (CurrentNormalAttacks < 0)
            CurrentNormalAttacks = 0;
    }

    //if they perform the action, will they exceed the limits
    public bool CanPerfromAttackAction()
    {
        if(CurrentNormalAttacks + CurrentSpecial1Attacks < MaxAttackActions)
            return true;
        return false;
    }

    public bool CanPerformNormal()
    {
        if (CurrentNormalAttacks < MaxNormalAttacks)
            return true;
        return false;
    }


}
