using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour {

    public Transform CombatCameraLocation;
    public Transform OOCCameraLocation;
    public Transform FinisherModeCameraLocation;

	public void MoveToCombatLocation()
    {
        //transform.position = CombatCameraLocation.position;
        StartCoroutine(moveToX(CombatCameraLocation, .4f));
    }

    public void MoveToOOCLocation()
    {
        //transform.position = OOCCameraLocation.position;
        StartCoroutine(moveToX(OOCCameraLocation, .4f));
    }

    public void MoveToFinisherModeLocation()
    {
        //transform.position = CombatCameraLocation.position;
        StartCoroutine(moveToX(FinisherModeCameraLocation, .2f));
    }

    bool isMoving = false;

    IEnumerator moveToX(Transform toLocation, float duration)
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

        while (counter < duration)
        {
            counter += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, toLocation.localPosition, counter / duration);
            transform.localRotation = Quaternion.Lerp(startRot, toLocation.localRotation, counter / duration);
            yield return null;
        }

        isMoving = false;
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

}
