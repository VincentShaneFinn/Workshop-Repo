using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class sceneload : MonoBehaviour {
    public string scene;
    public float delay;
	// Use this for initialization
	void Start () {
        StartCoroutine("loadscene");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator loadscene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(scene);
    }
}
