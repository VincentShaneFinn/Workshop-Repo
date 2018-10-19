using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour {

    //For debugging purposes
    public bool canDie = true;

    public int health = 100;
    public Slider healthSlider;
    public Text gameOverText;

    void Start()
    {
        healthSlider = GameObject.Find("/Canvas/Sliders/Health Slider").GetComponent<Slider>();
        gameOverText = GameObject.Find("/Canvas/Game Over").GetComponent<Text>();

        healthSlider.value = health;
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PlayerHit()
    {
        health -= 10;
        //Debug.Log("Health: " + health.ToString());
        healthSlider.value = health;

        if (health <= 0 && canDie == true)
        {
            gameOverText.text = "Game Over";
        }
    }

    public void PlayerHit(int damage)
    {
        health -= damage;

        healthSlider.value = health;

        if (health <= 0 && canDie == true)
        {
            gameOverText.text = "Game Over";
        }
    }
}
