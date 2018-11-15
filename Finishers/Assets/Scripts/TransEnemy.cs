using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransEnemy : MonoBehaviour {
    private Renderer[] renderers;
	// Use this for initialization
	void Start () {
        renderers = GetComponentsInChildren<Renderer>(true);
        stoptransparent();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void maketransparent(float t) {
        
        foreach(Renderer r in renderers) {
            Color c = r.material.color;
            c.a = t;
            r.material.color = c;
        }
    }
    public void stoptransparent() {
        foreach (Renderer r in renderers)
        {
            Color c = r.material.color;
            c.a = 1f;
            r.material.color = c;
        }
    }
}
