using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerCollider : MonoBehaviour {

    public PlayerMovementController player;
    public PlayerHealthController healthC;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "EnemySword")
        {
            StartCoroutine(player.KnockbackPlayer(col.transform.parent.gameObject));
            healthC.PlayerHit(col.gameObject.GetComponent<EnemySword>().damage);
        }
        if(col.gameObject.tag == "EnemyProjectileStraight")
        {
            StartCoroutine(player.KnockbackPlayer(col.transform.gameObject));
            healthC.PlayerHit(col.gameObject.GetComponent<EnemyProjectileStraight>().damage);
            col.gameObject.GetComponent<EnemyProjectileStraight>().HitPlayer();
        }
    }
}
