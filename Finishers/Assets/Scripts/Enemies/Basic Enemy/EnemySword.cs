using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySword : MonoBehaviour {
    
    public float duration; //duration is currently the only thing that turns off the attack
    private float count;
    public float delay;
    public int damage;
    public float eff=0;
    public PlayerMovementController playerM;
    private bool isAttacking;
    //Use this for initialization
    public void OnEnable()
    {
        StartCoroutine(trig(duration + delay));
        playerM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }
    private bool col;
    private void OnTriggerEnter(Collider collision)
    {
        col = true;

        if (collision.gameObject.CompareTag("PlayerModel"))
        {
            /* damage calculation
            if (collision.gameObject.GetComponents<>().Length > 0)
            {
                collision.gameObject.GetComponent<>().damage(damage);
            }
            */

            collision.gameObject.GetComponent<PlayerHealthController>().PlayerHit(damage);
            //StartCoroutine(playerM.KnockbackPlayer(transform.parent.gameObject));

            //hit back wont work this way
            //collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * eff);
            
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

    private IEnumerator trig(float n)
    {
        yield return new WaitForSeconds(n);
        gameObject.SetActive(false);
    }
}
