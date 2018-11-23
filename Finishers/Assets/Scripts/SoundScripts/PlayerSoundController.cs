using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour {

    public AudioClip[] PlayerHits;
    public AudioClip SiphonCut;
    public AudioClip Flamethrower;
    public AudioClip FlameAOE;

    public AudioClip[] PlayerRunicStabs; //up right down left;

    public AudioSource PlayerAS;
    
    public void PlayHitSound()
    {
        int n = Random.Range(0, PlayerHits.Length);
        PlayerAS.clip = PlayerHits[n];
        PlayerAS.Play();
    }

    public void PlayRunicStab(Direction buttonDir)
    {
        switch (buttonDir)
        {
            case Direction.up:
                PlayerAS.clip = PlayerRunicStabs[0];
                break;
            case Direction.right:
                PlayerAS.clip = PlayerRunicStabs[1];
                break;
            case Direction.down:
                PlayerAS.clip = PlayerRunicStabs[2];
                break;
            case Direction.left:
                PlayerAS.clip = PlayerRunicStabs[3];
                break;
        }
        PlayerAS.Play();
    }

    public void PlaySiphonCut()
    {
        PlayerAS.clip = SiphonCut;
        PlayerAS.Play();
    }

    public void PlayFlamethrower()
    {
        PlayerAS.clip = Flamethrower;
        PlayerAS.Play();
    }

    public void PlayFlameAOE()
    {
        PlayerAS.clip = FlameAOE;
        PlayerAS.Play();
    }
}
