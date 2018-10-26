using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class lets AI take an arc angle and give it back

public class ArcAngles {
    
    private List<float> angles;

    public ArcAngles() { MakeAnglesList(); }

    public void MakeAnglesList()
    {
        angles = new List<float>{-40f, -35f, -30f, -25f, -20f, -15f, -10f, -5f, 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f };
        ShuffleList();
    }

    public float TakeAngle()
    {
        int index = 0;
        float getAngle = angles[index];
        angles.RemoveAt(index);
        return angles[index];
    }

    public void ReturnAngle(float returnAngle)
    {
        angles.Add(returnAngle);
    }

    public List<float> GetList() { return angles; }

    private void ShuffleList()
    {
        for (int i = 0; i < angles.Count; i++)
        {
            float temp = angles[i];
            int randomIndex = Random.Range(i, angles.Count);
            angles[i] = angles[randomIndex];
            angles[randomIndex] = temp;
        }
    }
}
