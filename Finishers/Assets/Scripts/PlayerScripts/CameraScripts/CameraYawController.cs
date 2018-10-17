using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraYawController : MonoBehaviour {

    public Transform player;
    public float horizontalSensitivity;
    public float smoothSpeed = 0.125f;

    private float playerY;

    // Use this for initialization
    void Start()
    {
        playerY = player.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!Cursor.visible)
        {
            Vector3 desiredPosition = new Vector3(player.position.x, player.position.y - playerY, player.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.Rotate(0, Input.GetAxis("Mouse X") * horizontalSensitivity * Time.unscaledDeltaTime, 0, Space.World);
            if (Time.timeScale != 1)
            {
                player.rotation = transform.rotation;
            }
        }
    }
}
