using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLimb : MonoBehaviour {

	// Use this for initialization
	void Start () {
        firedPressed = false;
        FinMode = GameObject.FindGameObjectWithTag("Player").GetComponent<FinisherMode>();
	}

    //VERY QUICK THROW TOGETHER STUFF
    private bool firedPressed;
    public int KillLimit = 3;
    public List<GameObject> DeadBodies;
    private int CurrentlyKilledCount = 0;
    public LayerMask obstacleLayers;
    private GameObject line;
    private FinisherMode FinMode;
    private bool HitWall = false;
	
	// Update is called once per frame
	void Update () {
        if (firedPressed)
        {
            if(!HitWall)
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

    public void DestroyLine()
    {
        Destroy(line);
    }

    public Material LineMat;
    GameObject DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = LineMat;
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
                if (CurrentlyKilledCount < KillLimit)
                {
                    col.gameObject.GetComponent<EnemyAI>().KillEnemy();
                    CurrentlyKilledCount++;
                    FinMode.IncreaseFinisherMeter(PlayerDamageValues.Instance.SiphoningFinMeterFill);
                    DeadBodies[CurrentlyKilledCount - 1].SetActive(true);
                }
            }
        }
        else if (col.gameObject.tag == "TargetDummy")
        {
            if (firedPressed)
            {
                if (CurrentlyKilledCount < KillLimit)
                {
                    Destroy(col.gameObject);
                    CurrentlyKilledCount++;
                    FinMode.IncreaseFinisherMeter(PlayerDamageValues.Instance.SiphoningFinMeterFill);
                    DeadBodies[CurrentlyKilledCount - 1].SetActive(true);
                }
            }
        }
        else if (obstacleLayers.Contains(col.gameObject.layer))
        {
            if (firedPressed)
            {
                Destroy(this);
                HitWall = true;
            }
                

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
