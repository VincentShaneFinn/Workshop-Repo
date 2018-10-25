using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float CameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObj;
    public GameObject forwardContainer;
    Vector3 FollowPOS;
    public float clampAngle = 80.0f;
    public float inputSensitivity = 150.0f;
    public GameObject CameraObj;
    public GameObject PlayerObj;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    Quaternion localRotationJustY;



    // Use this for initialization
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStatus.GamePaused)
        {
            // We setup the rotation of the sticks here
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            finalInputX = mouseX;
            finalInputZ = -mouseY;

            //stop if paused
            rotY += finalInputX * inputSensitivity * Time.unscaledDeltaTime;
            rotX += finalInputZ * inputSensitivity * Time.unscaledDeltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            localRotationJustY = Quaternion.Euler(0f, rotY, 0.0f);
            transform.rotation = localRotation;
            forwardContainer.transform.rotation = localRotationJustY;
        }
    }

    void LateUpdate()
    {
        if (!GameStatus.GamePaused)
        {
            CameraUpdater();
        }
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = CameraMoveSpeed * Time.unscaledDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (Time.timeScale != 1) //if game paused
        {
            PlayerObj.transform.rotation = localRotationJustY;
        }
    }
}
