using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuCanvas; // Reference to the pause menu Canvas
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the pause menu canvas is hidden initially
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handle pause menu toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;

        // Hide the pause menu canvas
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    public void SwitchToMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    private void TogglePauseMenu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;

        // Show the pause menu canvas
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(true);
        }
    }
}
