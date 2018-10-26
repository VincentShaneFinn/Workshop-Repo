using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour {
    
    public int damage;

    //Use this for initialization
    public void OnEnable()
    {
    }
    private bool col;
    private void OnTriggerEnter(Collider collision)
    {
        col = true;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealthController>().PlayerHit(damage);
            
            //add enemy recoil when they connected with player
            //gameObject.SetActive(false);
        }
        /*movable objects
        if (collision.gameObject.CompareTag(""))
        {
            collision.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity * eff);
            Destroy(this.gameObject);
        }
        */
    }
}
