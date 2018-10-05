using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

    public Transform forwardObject;
    public GameObject PlayerModel;

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float rotateSpeed = 6;
    private Vector3 moveDirection = Vector3.zero;

    public CharacterController controller;

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            //transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(forwardObject.TransformDirection(moveDirection) * Time.deltaTime);

        //characterController.Move(forwardObject.TransformDirection(moveDirection) * Time.deltaTime * moveSpeed); // not physics friendly
        Vector3 movement = new Vector3(forwardObject.TransformDirection(moveDirection).x, 0.0f, forwardObject.TransformDirection(moveDirection).z);
        if (movement != Vector3.zero)
        {
            PlayerModel.transform.rotation = Quaternion.Lerp(PlayerModel.transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 20);
        }
    }
}
