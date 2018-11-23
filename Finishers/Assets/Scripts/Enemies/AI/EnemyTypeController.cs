using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { FireEnemy, IceEnemy}

public class EnemyTypeController : MonoBehaviour {

    public EnemyType MyEnemyType;
    public SkinnedMeshRenderer EnemySkin = null;
    public MeshRenderer DummySkin = null;
    public Material FireEnemyMat;
    public Material IceEnemyMat;

    // Use this for initialization
    void Start() {
        if (gameObject.tag == "TargetDummy")
        {
            switch (MyEnemyType)
            {
                case EnemyType.FireEnemy:
                    DummySkin.material = FireEnemyMat;
                    break;
                case EnemyType.IceEnemy:
                    DummySkin.material = IceEnemyMat;
                    break;
            }
        }
        else
        {
            switch (MyEnemyType)
            {
                case EnemyType.FireEnemy:
                    EnemySkin.material = FireEnemyMat;
                    break;
                case EnemyType.IceEnemy:
                    EnemySkin.material = IceEnemyMat;
                    break;
            }
        }
	}
}
