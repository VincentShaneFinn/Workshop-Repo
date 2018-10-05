using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTiltController : MonoBehaviour {

    public Transform player;
    public float verticalSensitivity;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localRotation.x <= .2 && transform.localRotation.x >= -.2)
        {
            transform.Rotate(-Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime, 0, 0, Space.Self);
        }
        if (transform.localRotation.x < -.2)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(-23f, 0, 0));

        }
        else if (transform.localRotation.x > .2)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(23f, 0, 0));
        }
        //print(transform.localRotation.eulerAngles);
    }
}
