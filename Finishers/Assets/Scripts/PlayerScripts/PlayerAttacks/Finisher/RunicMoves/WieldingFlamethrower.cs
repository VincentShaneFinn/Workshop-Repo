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
    private CameraFollow CF;
    private CameraMovementController CMC;

    // Use this for initialization
    void Start()
    {
        DestroyCount = 0;
        SpawnCount = 0;
        pmc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
        CF = GameObject.FindGameObjectWithTag("CameraBase").GetComponent<CameraFollow>();
        CMC = GameObject.FindGameObjectWithTag("CameraTarget").GetComponent<CameraMovementController>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSoundController>().PlayFlamethrower();
    }

    // Update is called once per frame
    void Update()
    {
        DestroyCount += Time.deltaTime;
        pmc.rotationSpeed = 1; //MARK: temporary slow player;
        pmc.walkSpeed = 2;
        CF.SetSensitivity(60);
        CMC.MoveToAimingLocation();
        //bool leftInput = Input.GetAxisRaw("Mouse X") > 0; // gets right
        //bool rightInput = Input.GetAxisRaw("Mouse Y") < 0; // gets left
        pmc.Aiming = true;
        if (DestroyCount > DestroyTime)
        {
            pmc.CanMove = true;
            pmc.Aiming = false;
            CF.RestoreSensitivity();
            CMC.ReturnFromAimingLocation();
            pmc.rotationSpeed = 60;
            pmc.walkSpeed = 6;
            Destroy(gameObject);
        }
        if (DestroyCount >= SpawnCount)
        {
            SpawnCount += FlameSpawnRate;
            Instantiate(MovingFlame, transform.position, transform.rotation);
        }
    }


}
