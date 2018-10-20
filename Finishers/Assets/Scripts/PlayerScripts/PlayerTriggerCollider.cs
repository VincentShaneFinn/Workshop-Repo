using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerCollider : MonoBehaviour {

    public PlayerMovementController player;
    public PlayerHealthController healthC;

    void OnTriggerEnter(Collider col)
    {
        //if(col.gameObject.tag == "Enemy")
        //{
        //    col.gameObject.GetComponent<EnemyMovementController>().PauseMovement();

        //    StartCoroutine(player.KnockbackPlayer(col.gameObject));

        //    healthC.PlayerHit();
        //}
        //else if
        if (col.gameObject.tag == "EnemySword")
        {
            StartCoroutine(player.KnockbackPlayer(col.transform.parent.gameObject));
        }
    }
}
