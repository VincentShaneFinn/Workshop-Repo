using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WieldingFlamethrower : MonoBehaviour {

    public GameObject MovingFlame;

    public float DestroyTime;
    public float FlameSpawnRate = .2f;
    private float DestroyCount;
    private float SpawnCount;
    private PlayerMovementController pmc;

    // Use this for initialization
    void Start()
    {
        DestroyCount = 0;
        SpawnCount = 0;
        pmc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        DestroyCount += Time.deltaTime;
        pmc.rotationSpeed = 1; //MARK: temporary slow player;
        pmc.CanMove = false;
        if (DestroyCount > DestroyTime)
        {
            pmc.CanMove = true;
            pmc.rotationSpeed = 60;
            Destroy(gameObject);
        }
        if (DestroyCount >= SpawnCount)
        {
            SpawnCount += FlameSpawnRate;
            Instantiate(MovingFlame, transform.position, transform.rotation);
        }
    }


}
