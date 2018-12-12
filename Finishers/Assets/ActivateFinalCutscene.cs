using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateFinalCutscene : MonoBehaviour {

    [SerializeField] GameObject videoPlayer;
    [SerializeField] Image imageToDim;

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
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
