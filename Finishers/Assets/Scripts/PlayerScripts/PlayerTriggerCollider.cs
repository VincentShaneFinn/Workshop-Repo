using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerCollider : MonoBehaviour {

    public PlayerMovementController player;
    private float speed = 12;

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyMovementController>().PauseMovement();
            Vector3 dir = (player.transform.position - col.transform.position).normalized;
            //StartCoroutine(player.KnockbackPlayer(dir, speed, .15f));
        }
    }
}
