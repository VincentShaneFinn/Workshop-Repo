using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour {

    public Transform CombatCameraLocation;
    public Transform OOCCameraLocation;
    public Transform FinisherModeCameraLocation;

    private Transform currentTargetLocation;
    private float currentSpeed;


    void Start()
    {

    }

    //void Update()
    //{
    //    if (isMoving)
    //    {
    //        float counter = 0;

    //        //Get the current position of the object to be moved
    //        Vector3 startPos = transform.localPosition;
    //        Quaternion startRot = transform.localRotation;

    //        if (startPos != currentTargetLocation.localPosition && startRot != currentTargetLocation.localRotation)
    //        {
    //            transform.localPosition = Vector3.Lerp(startPos, currentTargetLocation.localPosition, currentSpeed * Time.deltaTime);
    //            transform.localRotation = Quaternion.Lerp(startRot, currentTargetLocation.localRotation, currentSpeed * Time.deltaTime);
    //        }

    //        isMoving = false;
    //    }
    //}

    public void MoveToCombatLocation()
    {
        //transform.position = CombatCameraLocation.position;

        currentTargetLocation = CombatCameraLocation;
        currentSpeed = .4f;
        CallCoroutineHelper();
    }

    public void MoveToOOCLocation()
    {
        //transform.position = OOCCameraLocation.position;

        currentTargetLocation = OOCCameraLocation;
        currentSpeed = .4f;
        CallCoroutineHelper();
    }

    public void MoveToFinisherModeLocation()
    {
        //transform.position = CombatCameraLocation.position;

        currentTargetLocation = FinisherModeCameraLocation;
        currentSpeed = .2f;
        CallCoroutineHelper();
    }

    Coroutine co;

    public void CallCoroutineHelper()
    {
        // stop the coroutine
        if(co != null)
            StopCoroutine(co);

        IEnumerable move = moveToCurrentTarget();

        // start the coroutine:
        co = StartCoroutine(move.GetEnumerator());
    }

    bool isMoving = false;

    IEnumerable moveToCurrentTarget()
    {
        //Make sure there is only one instance of this function running
        if (isMoving)
        {
            //yield break; ///exit if this is still running
        }
        isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        while (counter < currentSpeed)
        {
            counter += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, currentTargetLocation.localPosition, counter / currentSpeed);
            transform.localRotation = Quaternion.Lerp(startRot, currentTargetLocation.localRotation, counter / currentSpeed);
            yield return null;
        }

        isMoving = false;
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

}
