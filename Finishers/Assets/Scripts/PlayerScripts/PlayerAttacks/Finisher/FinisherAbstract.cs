using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FinisherAbstract : MonoBehaviour {
    public List<Direction> keylist;
	public abstract void startfinisher(FinisherMode f);
    public bool check(List<Direction> inputs) {

        if (inputs.Count < keylist.Count) {
            return false;
        }

        //checks from end
        int j = inputs.Count - keylist.Count;
        for (int i=keylist.Count-1;i>=0;i--) {
            if (!inputs[i + j].Equals(keylist[i])) {
                return false;
            }
        }
        return true;
    }
    public bool startfinisher(FinisherMode fm,List<Direction> input) {
        bool goodCombo = check(input);
        if (goodCombo) {
            startfinisher(fm);
        }
        return goodCombo;
    }
}