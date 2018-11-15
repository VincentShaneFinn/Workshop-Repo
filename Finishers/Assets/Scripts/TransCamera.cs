using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransCamera : MonoBehaviour
{
    public float alpha = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if ((collision.gameObject.gameObject.GetComponent<TransEnemy>()) != null)
            {
                TransEnemy e = collision.gameObject.GetComponent<TransEnemy>();
                e.maketransparent(alpha);
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if ((collision.gameObject.gameObject.GetComponent<TransEnemy>()) != null)
            {
                TransEnemy e = collision.gameObject.GetComponent<TransEnemy>();
                e.stoptransparent();
            }
        }
    }
}
