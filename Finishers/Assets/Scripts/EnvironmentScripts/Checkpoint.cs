using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public GameStatus gm;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            gm.CheckpointP = transform.position;
            gm.SaveGame();
        }
    }
}
