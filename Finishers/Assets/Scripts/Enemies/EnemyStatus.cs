using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior {

    private GameObject Enemy;
    private EnemyBehaviorStatus Status;

    public EnemyBehavior() { }

    public EnemyBehavior(GameObject e, EnemyBehaviorStatus s)
    {
        Enemy = e;
        s = Status;
    }

    public GameObject GetEnemy() { return Enemy; }
    public void SetEnemy(GameObject e) { Enemy = e; }

    public EnemyBehaviorStatus GetStatus() { return Status; }
    public void SetStatus(EnemyBehaviorStatus s) { Status = s; }

}
