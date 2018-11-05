using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RunicInputHelper
{
    private List<Direction> Que;
    public List<Direction> FireCombo = new List<Direction>() { Direction.left, Direction.up, Direction.down };
    public List<Direction> IceCombo = new List<Direction>() { Direction.right, Direction.up, Direction.down };

    public RunicInputHelper() { RestartQue(); }

    public int GetCount()
    {
        return Que.Count;
    }

    public bool AddInput(Direction d)
    {
        Que.Add(d);

        if (GoodSoFar(FireCombo))
        {
            return true;
        }

        if (GoodSoFar(IceCombo))
        {
            return true;
        }

        return false;
    }

    public void RestartQue()
    {
        Que = new List<Direction>();
    }

    private bool GoodSoFar(List<Direction> combo)
    {
        if (Que.Count > combo.Count)
            return false;
        bool goodCombo = true;
        for (int i = 0; i < Que.Count; i++)
        {
            if (Que[i] == combo[i])
            {
                goodCombo = true;
            }
            else
            {
                goodCombo = false;
                break;
            }
        }
        return goodCombo;
    }

    public bool SuccessfulCombo(List<Direction> combo)
    {
        if(Que.Count != combo.Count)
        {
            return false;
        }
        bool goodCombo = true;
        for (int i = 0; i < combo.Count; i++)
        {
            if (Que[i] == combo[i])
            {
                goodCombo = true;
            }
            else
            {
                goodCombo = false;
                break;
            }
        }
        return goodCombo;
    }
}
