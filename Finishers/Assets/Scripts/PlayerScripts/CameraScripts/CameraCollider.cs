using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour {

    public Transform CameraBase;
    public Transform CameraTarget;
    public Transform[] posCheckArr;

    public LayerMask collisionLayer;

    private Vector3 currentTargetLocation;
    private bool colliding;

    // Use this for initialization
    void Start () {
        colliding = false;
        currentTargetLocation = CameraTarget.position;
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;

        var heading = CameraTarget.position - CameraBase.position;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(CameraBase.position, direction, out hit, distance, collisionLayer))
        {
            Debug.DrawRay(CameraBase.position, direction * (hit.distance - .387f), Color.yellow);
            if (hit.distance > 2)
                transform.position = hit.point;
            return;
        }

        //for (int i = 0; i < posCheckArr.Length; i++)
        //{
        //    RaycastHit hit2;
        //    var headingLoop = posCheckArr[i].position - CameraBase.position;
        //    var distanceLoop = headingLoop.magnitude;
        //    var directionLoop = headingLoop / distanceLoop; // This is now the normalized direction.
        //    Debug.DrawLine(CameraBase.position, posCheckArr[i].position, Color.red);
        //    // Does the ray intersect any objects excluding the player layer
        //    if (Physics.Raycast(CameraBase.position, directionLoop, out hit2, distance, collisionLayer))
        //    {
        //        Debug.DrawRay(CameraBase.position, directionLoop * (hit2.distance - .387f), Color.yellow);
        //        if (hit2.distance > 2)
        //            transform.position = CameraBase.position + direction * (hit2.distance - .387f);
        //        return;
        //    }
        //}

        transform.position = CameraTarget.position;
    }
}
