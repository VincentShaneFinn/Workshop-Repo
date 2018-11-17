using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLimb : MonoBehaviour {

	// Use this for initialization
	void Start () {
        firedPressed = false;
	}

    //VERY QUICK THROW TOGETHER STUFF
    private bool firedPressed;
    public int KillLimit = 3;
    private int CurrentlyKilledCount = 0;
    public LayerMask obstacleLayers;
    private GameObject line;
	
	// Update is called once per frame
	void Update () {
        if (firedPressed)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 20);
        }
	}

    public void ButtonHeld()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "TargetDummy")
            {
                if (line != null)
                    Destroy(line);
                line = DrawLine(transform.position, hit.point, Color.red);
            }
            else
            {
                if (line != null)
                    Destroy(line);
                line = DrawLine(transform.position, hit.point, Color.white);
            }
        }
        else
        {
            if (line != null)
                Destroy(line);
            line = DrawLine(transform.position, transform.position + transform.forward * 20, Color.white);
        }
    }

    public void ButtonReleased()
    {
        firedPressed = true;
        transform.parent = null;
        Destroy(line);
    }

    GameObject DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.05f, 0.05f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (firedPressed)
            {
                if (col.gameObject.GetComponent<EnemyAI>().CurrentStatus != EnemyBehaviorStatus.Sleeping)
                {
                    col.gameObject.GetComponent<EnemyAI>().KillEnemy();
                    CurrentlyKilledCount++;
                    if(CurrentlyKilledCount >= KillLimit)
                        Destroy(gameObject);
                }
            }
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            if (firedPressed)
            {
                Destroy(col.gameObject);
                CurrentlyKilledCount++;
                if (CurrentlyKilledCount >= KillLimit)
                    Destroy(gameObject);
            }
        }
        else if (obstacleLayers.Contains(col.gameObject.layer))
        {
            if(firedPressed)
                Destroy(gameObject);
        }
    }
}

public static class UnityExtensions
{

    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
