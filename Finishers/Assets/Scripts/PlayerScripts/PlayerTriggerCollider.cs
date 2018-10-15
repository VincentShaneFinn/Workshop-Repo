using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerCollider : MonoBehaviour {

    public PlayerMovementController player;
    private float speed = 12;

    //For debugging purposes
    public bool canDie = true;

    public int health = 100;
    public Slider healthSlider;
    public Text gameOverText;

    void Start()
    {
        healthSlider.value = health;
        gameOverText.text = "";
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyMovementController>().PauseMovement();
            Vector3 dir = (player.transform.position - col.transform.position).normalized;
            StartCoroutine(player.KnockbackPlayer(dir, speed, .15f));


            health -= 10;
            Debug.Log("Health: " + health.ToString());
            healthSlider.value = health;

            if(health <= 0 && canDie == true)
            {
                gameOverText.text = "Game Over";
            }
        }
    }
}
