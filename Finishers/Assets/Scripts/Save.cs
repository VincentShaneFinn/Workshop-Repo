using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save {
    public List<int> DeadGroups = new List<int>();
    public float playerX = 0;
    public float playerY = 1;
    public float playerZ = 0;

    public float healthMeter = 0;
    public float finisherMeter = 0;

}
