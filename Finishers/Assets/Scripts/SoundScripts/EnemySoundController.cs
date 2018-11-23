using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundController : MonoBehaviour {

    public AudioClip[] enemyHits;
    public AudioClip enemyFootstep;

    public AudioSource enemyAS;

    public void PlayFootstep()
    {
        enemyAS.clip = enemyFootstep;
        enemyAS.Play();
    }

    public void PlayEnemyHit()
    {
        int n = Random.Range(0, enemyHits.Length);
        enemyAS.clip = enemyHits[n];
        enemyAS.Play();
    }
}
