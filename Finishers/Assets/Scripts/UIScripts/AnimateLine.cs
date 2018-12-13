using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLine : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        StartCoroutine(ScaleUp());
	}
	
    IEnumerator ScaleUp()
    {
        float time = .15f;
        float count = 0;
        while(count < time)
        {
            count += Time.unscaledDeltaTime;
            yield return null;
            transform.localScale = new Vector3(1 * count / time, 1, 1);
        }
        transform.localScale = new Vector3(1, 1, 1);
    }
}
