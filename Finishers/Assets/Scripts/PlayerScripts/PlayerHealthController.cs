using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{

    //For debugging purposes
    public bool canDie = true;

    public float MaxHealth = 100;
    public Slider healthSlider;
    public Text gameOverText;
    public PlayerUpdater pUpdater;
    public GameStatus gm;

    void Start()
    {
        healthSlider.value = MaxHealth;
        gameOverText.text = "";
    }

    public void PlayerHit(float damage)
    {
        if (pUpdater.ImmuneCount < pUpdater.ImmuneTime || GetComponent<PlayerMovementController>().isDashing() || GameStatus.FinisherModeActive)
            return;
        else
            pUpdater.ImmuneCount = 0;

        MaxHealth -= damage;

        healthSlider.value = MaxHealth;

        if (MaxHealth <= 0 && canDie == true)
        {
            gameOverText.text = "Game Over";
            gm.PlayerDied();
            //gm.LoadGame();
          
        }
    }

    public void PlayerHealed(float health)
    {
        MaxHealth += health;
        if (MaxHealth > 100)
        {
            MaxHealth = 100;
        }
        healthSlider.value = MaxHealth;
    }
}
