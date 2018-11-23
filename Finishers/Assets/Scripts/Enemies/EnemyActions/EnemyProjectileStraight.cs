using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileStraight : MonoBehaviour {

    public float speed = 10f;
    public float damage = 10f;
    public LayerMask obstacles;

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public void HitPlayer()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col) //destroy when hit wall
    {
        if(obstacles.Contains(col.gameObject.layer)){
            Destroy(gameObject);
        }
    }

}
