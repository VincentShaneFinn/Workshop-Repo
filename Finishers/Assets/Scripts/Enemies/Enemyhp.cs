using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { NormalBlade, HeavyBlade, Fire, Frost }

public class Enemyhp : MonoBehaviour {
    public float currenthp;
    public float hp = 100;
    public GameObject BloodTrail;
    public GameObject IceTrail;
    public EnemyTypeController etc;
    public int RestancePercentage = 30;
    public Material lowRed;
    public Material lowBlue;
    public Material lowNormal;

	// Use this for initialization
	void Start () {
        currenthp = hp;
	}

    void checkhp() {
        var randomRotation = Quaternion.Euler(transform.rotation.x, Random.Range(0, 360), transform.rotation.z);
        GameObject blood = Instantiate(BloodTrail, transform.position, randomRotation);
        Destroy(blood, 1);

        if (GetComponent<EnemyConditionManager>().CurrentCondition == EnemyConditions.Frozen)
        {
            FrozenEnemyHit();
            return;
        }
        
        if(currenthp <= 33)
        {
            if(gameObject.tag != "TargetDummy")
            {
                if (etc.MyEnemyType == EnemyType.FireEnemy)
                    etc.SetLowHealthSkin(lowRed);
                else if (etc.MyEnemyType == EnemyType.IceEnemy)
                    etc.SetLowHealthSkin(lowBlue);
                else
                    etc.SetLowHealthSkin(lowNormal);
            }
            else
            {
                if (etc.MyEnemyType == EnemyType.FireEnemy)
                    etc.DummySkin.material = lowRed;
                else
                    etc.DummySkin.material = lowBlue;
            }
        }

        if (currenthp<=0) {
            if (gameObject.tag != "TargetDummy")
                GetComponent<EnemyAI>().KillEnemy();
            else
                Destroy(gameObject);
        }
    }

    public void damage(float d, AttackType type) {
        if(etc.MyEnemyType == EnemyType.FireEnemy)
        {
            if(type == AttackType.Fire)
            {
                d = (int)(d * ((100 - RestancePercentage) / 100f));
            }
            else if(type == AttackType.Frost)
            {
                d = (int)(d * ((100 + RestancePercentage) / 100f));
            }
        }
        else if(etc.MyEnemyType == EnemyType.IceEnemy)
        {
            if (type == AttackType.Frost)
            {
                d = (int)(d * ((100 - RestancePercentage) / 100f));
            }
            else if (type == AttackType.Fire)
            {
                d = (int)(d * ((100 + RestancePercentage) / 100f));
            }
        }

        //dont damage boss
        if(etc.MyEnemyType != EnemyType.Boss)
            currenthp -= d;

        checkhp();
    }

    public void FrozenEnemyHit()
    {
        // Gets a vector that points from the player's position to the target's.
        print(transform.position);
        print(GameObject.FindGameObjectWithTag("Player").transform.position);
        var heading = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
        print(heading);
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.

        GameObject iceBurst = Instantiate(IceTrail, transform.position, Quaternion.LookRotation(direction));
        Destroy(iceBurst, 1);
        Destroy(gameObject);
    }
}
