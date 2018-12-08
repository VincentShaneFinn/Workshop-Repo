using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { FireEnemy, IceEnemy, Boss}

public class EnemyTypeController : MonoBehaviour {

    public EnemyType MyEnemyType;
    public SkinnedMeshRenderer EnemySkin = null;
    public MeshRenderer DummySkin = null;
    public Material FireEnemyMat;
    public Material IceEnemyMat;

    public bool isNewKnight = false;
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer belt;
    public SkinnedMeshRenderer[] shoulders;

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
                default:
                    DummySkin.material = FireEnemyMat;
                    break;
            }
        }
        else if(isNewKnight)
        {
            switch (MyEnemyType)
            {
                case EnemyType.FireEnemy:
                    //body.material = FireEnemyMat;
                    belt.material = FireEnemyMat;
                    shoulders[0].material = FireEnemyMat;
                    shoulders[1].material = FireEnemyMat;
                    break;
                case EnemyType.IceEnemy:
                    //body.material = IceEnemyMat;
                    belt.material = IceEnemyMat;
                    shoulders[0].material = IceEnemyMat;
                    shoulders[1].material = IceEnemyMat;
                    break;
                default:
                    belt.material = FireEnemyMat;
                    shoulders[0].material = FireEnemyMat;
                    shoulders[1].material = FireEnemyMat;
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
                default:
                    EnemySkin.material = FireEnemyMat;
                    break;
            }
        }
	}

    public void SetLowHealthSkin(Material mat)
    {
        if (isNewKnight)
        {
            belt.material = mat;
            shoulders[0].material = mat;
            shoulders[1].material = mat;
        }
        else
        {
            EnemySkin.material = mat;
        }
    }
}
