using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour {

    public Transform CombatCameraLocation;
    public Transform OOCCameraLocation;
    public Transform FinisherModeCameraLocation;
    public Transform AimingCameraLocation;

    public PlayerMovementController PMC;

    private Transform currentTargetLocation;
    private float currentSpeed;

    public void SwitchCombatLocation()
    {
        //transform.position = OOCCameraLocation.position;
        if (currentTargetLocation != AimingCameraLocation)
        {
            if (GameStatus.InCombat)
            {
                currentTargetLocation = CombatCameraLocation;
            }
            else
            {
                currentTargetLocation = OOCCameraLocation;
            }
            currentSpeed = .4f;
            CallCoroutineHelper();
        }
    }

    public void MoveToFinisherModeLocation()
    {
        //transform.position = CombatCameraLocation.position;

        currentTargetLocation = FinisherModeCameraLocation;
        currentSpeed = .45f;
        CallCoroutineHelper();
    }

    //We will need to test these next two
    public void MoveToAimingLocation()
    {
        PMC.Aiming = true;
        currentTargetLocation = AimingCameraLocation;
        currentSpeed = .1f;
        CallCoroutineHelper();
    }
    public void ReturnFromAimingLocation()
    {
        PMC.Aiming = false;
        if (GameStatus.InCombat)
            currentTargetLocation = CombatCameraLocation;
        else
            currentTargetLocation = OOCCameraLocation;
        currentSpeed = .1f;
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

    //public DebugSettings debug = new DebugSettings();
    //public CollisionHandler collision = new CollisionHandler();
    //Vector3 adjustedDestination = Vector3.zero;
    //Vector3 camVel = Vector3.zero;

    //void Start()
    //{
    //    currentTargetLocation = OOCCameraLocation;
    //    collision.Initialize(Camera.main);
    //    collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
    //    collision.UpdateCameraClipPoints(currentTargetLocation.position, transform.rotation, ref collision.desiredCameraClipPoints);

    //}

    //void FixedUpdate()
    //{
    //    collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
    //    collision.UpdateCameraClipPoints(currentTargetLocation.position, transform.rotation, ref collision.desiredCameraClipPoints);

    //    //draw debug lines
    //    for (int i = 0; i < 5; i++)
    //    {
    //        if (debug.drawAdjustedCollisionLines)
    //        {
    //            Debug.DrawLine(currentTargetLocation.position, collision.adjustedCameraClipPoints[i], Color.green);
    //        }
    //        if (debug.drawDesiredCollisionLines)
    //        {
    //            Debug.DrawLine(currentTargetLocation.position, collision.desiredCameraClipPoints[i], Color.white);
    //        }
    //    }

    //    collision.CheckColliding(currentTargetLocation.position);
    //    //position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(currentTargetLocation.position);
    //}

    //[System.Serializable]
    //public class DebugSettings
    //{
    //    public bool drawDesiredCollisionLines = true;
    //    public bool drawAdjustedCollisionLines = true;
    //}

    //[System.Serializable]
    //public class CollisionHandler
    //{
    //    public LayerMask collisionLayer;

    //    [HideInInspector]
    //    public bool colliding = false;
    //    public Vector3[] adjustedCameraClipPoints;
    //    public Vector3[] desiredCameraClipPoints;

    //    Camera camera;

    //    public void Initialize(Camera cam)
    //    {
    //        camera = cam;
    //        adjustedCameraClipPoints = new Vector3[5];
    //        desiredCameraClipPoints = new Vector3[5];
    //    }

    //    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    //    {
    //        if (!camera)
    //            return;

    //        //clear contents
    //        intoArray = new Vector3[5];

    //        float z = camera.nearClipPlane;
    //        float x = Mathf.Tan(camera.fieldOfView / 3.41f);
    //        float y = x / camera.aspect;

    //        //top left
    //        intoArray[0] = (atRotation * new Vector3(-x,y,z)) + cameraPosition;
    //        //top right
    //        intoArray[1] = (atRotation * new Vector3(x, y, -z)) + cameraPosition;
    //        //bottom left
    //        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
    //        //bottom right
    //        intoArray[3] = (atRotation * new Vector3(x, -y, -z)) + cameraPosition;
    //        //camera's position
    //        intoArray[4] = cameraPosition - camera.transform.forward;
    //    }

    //    //pass adjusted for my stuff?
    //    bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    //    {
    //        for (int i = 0; i < clipPoints.Length; i++)
    //        {
    //            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
    //            float distance = Vector3.Distance(clipPoints[i], fromPosition);
    //            if (Physics.Raycast(ray, distance, collisionLayer))
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public float GetAdjustedDistanceWithRayFrom(Vector3 from)
    //    {
    //        float distance = 1;

    //        for(int i = 0; i < desiredCameraClipPoints.Length; i++)
    //        {
    //            Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
    //            RaycastHit hit;
    //            if(Physics.Raycast(ray,out hit))
    //            {
    //                if (distance == 1)
    //                    distance = hit.distance;
    //                else
    //                {
    //                    if (hit.distance < distance)
    //                        distance = hit.distance;
    //                }
    //            }
    //        }

    //        if (distance == 1)
    //            return 0;
    //        else
    //            return distance;
    //    }

    //    public void CheckColliding(Vector3 targetPosition)
    //    {
    //        if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
    //        {
    //            colliding = true;
    //        }
    //        else
    //        {
    //            colliding = false;
    //        }
    //    }
    //}
}
