using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordSoundController : MonoBehaviour {

    public bool Attack1 = false;
    public bool Attack2 = false;
    private bool didAttack = false;
    private bool foot1Last = false;
    public AudioSource Attack1Source;
    public AudioSource Attack2Source;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Attack1 == true || Attack2 == true)
        {
            if (!didAttack)
            {
                if (Attack1)
                    Attack1Source.Play();
                else
                    Attack2Source.Play();
                didAttack = true;
            }
            Attack1 = false;
            Attack2 = false;
        }
        else
        {
            didAttack = false;
        }
    }
}
