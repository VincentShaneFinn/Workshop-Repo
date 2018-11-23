using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyhp : MonoBehaviour {
    public float currenthp;
    public float hp = 100;
    public GameObject BloodTrail;
    public GameObject IceTrail;

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

        if (currenthp<=0) {
            if (gameObject.tag != "TargetDummy")
                GetComponent<EnemyAI>().KillEnemy();
            else
                Destroy(gameObject);
        }
    }

    public void damage(float d) {
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
