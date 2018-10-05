using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {

    public Transform forwardObject;
    public float moveSpeed;
    public float jumpVelocity;
    public GameObject PauseMenu;

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float rotateSpeed = 6;
    private Vector3 moveDirection = Vector3.zero;

    public CharacterController controller;

    // Use this for initialization
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            SetCursorState();
            return;
        }

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
        controller.Move(moveDirection * Time.deltaTime);

        //characterController.Move(forwardObject.TransformDirection(moveDirection) * Time.deltaTime * moveSpeed); // not physics friendly
    }

    // Apply requested cursor state
    void SetCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        // Hide cursor when locking
        Cursor.visible = true;
    }
}
