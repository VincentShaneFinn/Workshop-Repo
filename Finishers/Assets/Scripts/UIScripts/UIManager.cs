using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject PauseMenu;

    // Use this for initialization
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //PauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            SetCursorState();
            return;
        }
    }

    // Apply requested cursor state
    void SetCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        // Hide cursor when locking
        Cursor.visible = true;
    }
}
