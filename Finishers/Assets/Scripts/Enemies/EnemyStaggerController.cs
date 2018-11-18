using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaggerController : MonoBehaviour {

    //If the enemy is staggered 3 times in a row, prevent knockback for x seconds
    //add an attack to the list, if > 3 dont stagger, if > 3. if not staggered for x seconds, reset list

    //MARK: improve this later by adding some visual or special move the enemy performs if poise becomes active

    EnemyMovementController enemyMovement;
    public float resetListTime = 3f;
    public float resetListCount = 3f;
    public bool poiseActive = false; // true means cannot be staggered
    public int staggerCount = 0;
    public int staggerLimit = 3;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovementController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (resetListCount < resetListTime)
        {
            if (staggerCount >= staggerLimit)
                poiseActive = true;
            else
                poiseActive = false;
        }
        else
        {
            poiseActive = false;
            staggerCount = 0;
        }
        resetListCount += Time.deltaTime;
	}

    //when hit by playersword, knockback, but we need to limit this
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "PlayerSwordTrigger")
        {
            if (!poiseActive)
            {
                enemyMovement.HelpKnockback();
                staggerCount++;
                resetListCount = 0;
            }
            else if(GetComponent<EnemyAI>().GetCurrentStatus() != EnemyBehaviorStatus.Attacking)
            {
                GetComponent<KnightEnemyActions>().PerformUnblockableAttack();
            }
        }
    }
}
