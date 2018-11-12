using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInNSeconds : MonoBehaviour {


    public float DestroyTime;
    private float DestroyCount;

    void Start()
    {
        DestroyCount = DestroyTime;
    }

    void Update()
    {
        DestroyCount -= Time.deltaTime;
        if(DestroyCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
