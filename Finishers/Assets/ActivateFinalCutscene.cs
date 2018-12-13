using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateFinalCutscene : MonoBehaviour {

    [SerializeField] GameObject videoPlayer;
    [SerializeField] Image imageToDim;
    public Canvas audio;
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            AudioSource[] a = audio.GetComponents<AudioSource>();
            foreach (AudioSource a2 in a) { a2.enabled = false; }
            StartCoroutine(Quit());
        }
    }

    IEnumerator Quit()
    {
        imageToDim.gameObject.SetActive(true);
        float alphaCount = 0;
        float totalTime = 2;
        while(alphaCount < totalTime)
        {
            imageToDim.color = new Color(0, 0, 0, alphaCount / totalTime);
            alphaCount += Time.deltaTime;
            yield return null;
        }
        videoPlayer.SetActive(true);
        yield return new WaitForSeconds(.2f);
        imageToDim.gameObject.SetActive(false);
        yield return new WaitForSeconds(8f);
        print("Quit Game");
        Application.Quit();
    }
}
