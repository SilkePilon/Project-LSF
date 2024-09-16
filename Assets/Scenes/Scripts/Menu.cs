using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;
public class CameraSwitch : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;

    public

    void Start()
    {

        // Add a listener to the play button
        if (playButton != null)
        {
            playButton.onClick.AddListener(SwitchToGameScene);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Play button not assigned in the inspector!");
        }
    }

    void SwitchToGameScene()
    {
        SceneManager.LoadScene(sceneName:"game");
    }

    void ExitGame()
    {
        Application.Quit();
        
    }
}