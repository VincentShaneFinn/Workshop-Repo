using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject PauseMenu;

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameStatus.FinisherModeActive)
        {
            PauseMenu.SetActive(true);
            GameStatus.GamePaused = true;
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
        PauseMenu.SetActive(false);
        GameStatus.GamePaused = false;
        if (!GameStatus.FinisherModeActive)
            Time.timeScale = 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
