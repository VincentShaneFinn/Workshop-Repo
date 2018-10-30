using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{

    //For debugging purposes
    public bool canDie = true;

    public int MaxHealth = 100;
    public Slider healthSlider;
    public Text gameOverText;
    public PlayerUpdater pUpdater;

    void Start()
    {
        healthSlider.value = MaxHealth;
        gameOverText.text = "";
    }

    public void PlayerHit()
    {
        PlayerHit(10);
    }

    public void PlayerHit(int damage)
    {
        if (pUpdater.ImmuneCount < pUpdater.ImmuneTime)
            return;
        else
            pUpdater.ImmuneCount = 0;

        MaxHealth -= damage;

        healthSlider.value = MaxHealth;

        if (MaxHealth <= 0 && canDie == true)
        {
            gameOverText.text = "Game Over";
        }
    }
}
