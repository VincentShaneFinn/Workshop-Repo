﻿using System.Collections;
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
    public GameObject PlayerModel;
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

    [SerializeField] float autoScrollTime = 2f;
    [SerializeField] float autoScrollCount = 0;



    // Use this for initialization
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SavedSens = inputSensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameStatus.GamePaused && !GameStatus.FinisherModeActive)
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
            if(Mathf.Abs(mouseX) <= Mathf.Epsilon && Mathf.Abs(mouseY) <= Mathf.Epsilon)
            {
                //Vector3 playerEulers = PlayerModel.transform.eulerAngles;
                localRotation = AutoCamRot(localRotation);
            }
            else
            {
                autoScrollCount = 0;
            }
            autoScrollCount += Time.deltaTime;

            transform.rotation = localRotation;
            forwardContainer.transform.rotation = localRotationJustY;
        }
        if (!GameStatus.GamePaused)
        {
            CameraUpdater();
        }
    }

    private Quaternion AutoCamRot(Quaternion localRotation)
    {
        if (autoScrollCount > autoScrollTime)
        {
            float rotateDirection = (((PlayerModel.transform.eulerAngles.y - transform.eulerAngles.y) + 360f) % 360f) > 180.0f ? -1 : 1;
            Vector3 cross = Vector3.Cross(transform.rotation * Vector3.forward, PlayerModel.transform.rotation * Vector3.forward); // ideally we want to get this to be 1
            if (Mathf.Abs(cross.y) > .2f)
            {
                if (cross.y < Mathf.Epsilon)
                {
                    //rotX += 1;
                    rotY -= Mathf.Abs(cross.y * 100f * Time.deltaTime);
                }
                else
                {
                    rotY += Mathf.Abs(cross.y * 100f * Time.deltaTime);
                }
            }
            else if (((PlayerModel.transform.eulerAngles.y - transform.eulerAngles.y) + 360f) % 360f > 160 && ((PlayerModel.transform.eulerAngles.y - transform.eulerAngles.y) + 360f) % 360f < 200)
            {
                rotY += 1f;
            }
            localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            localRotationJustY = Quaternion.Euler(0f, rotY, 0.0f);
        }

        return localRotation;
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = CameraMoveSpeed * Time.unscaledDeltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        transform.position = target.position;

        if (Time.timeScale != 1 && !GameStatus.FinisherModeActive) //if game paused
        {
            //used for aiming
            PlayerModel.transform.rotation = localRotationJustY;
        }
    }

    public void SetSensitivity(float val)
    {
        inputSensitivity = val;
    }
    private float SavedSens;
    public void RestoreSensitivity()
    {
        inputSensitivity = SavedSens;
    }
}
