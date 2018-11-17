using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public AudioSource OOCMusicSource;
    public AudioSource InCombatMusicSource;

    private bool previousGameStatus = false;
    public float TransitionDelay;
    public float desiredGameVolume = .1f;

    void Start()
    {
        OOCMusicSource.volume = desiredGameVolume;
        InCombatMusicSource.volume = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if(GameStatus.InCombat != previousGameStatus)
        {
            if (GameStatus.InCombat)
            {
                StartCoroutine(SwitchToInCombat());
            }
            else
            {
                StartCoroutine(SwitchToOOCombat());
            }
            previousGameStatus = GameStatus.InCombat;
        }
	}

    IEnumerator SwitchToInCombat()
    {
        print("test");
        float delay = TransitionDelay;
        while(delay > 0)
        {
            OOCMusicSource.volume = desiredGameVolume * (delay / TransitionDelay);
            InCombatMusicSource.volume = desiredGameVolume * ((TransitionDelay - delay) / TransitionDelay);
            delay -= Time.unscaledDeltaTime;
            yield return null;
        }
        OOCMusicSource.volume = 0;
        InCombatMusicSource.volume = desiredGameVolume;
    }

    IEnumerator SwitchToOOCombat()
    {
        float delay = TransitionDelay;
        while (delay > 0)
        {
            OOCMusicSource.volume = desiredGameVolume * ((TransitionDelay - delay) / TransitionDelay);
            InCombatMusicSource.volume = desiredGameVolume * (delay / TransitionDelay);
            delay -= Time.unscaledDeltaTime;
            yield return null;
        }
        OOCMusicSource.volume = desiredGameVolume;
        InCombatMusicSource.volume = 0;
    }
}
