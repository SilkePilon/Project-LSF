using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraSwitch : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;
    public Button settingsButton;
    public Animation settingsAnimation;
    public Animation benchAnimation;

    private bool isSettingsOpen = false;

    void Start()
    {
        SetupButton(playButton, SwitchToGameScene, "Play");
        SetupButton(exitButton, ExitGame, "Exit");
        SetupButton(settingsButton, ToggleSettingsMenu, "Settings");

        CheckAnimationComponent(settingsAnimation, "Settings");
        CheckAnimationComponent(benchAnimation, "Bench");
    }

    void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
        else
        {
            Debug.LogWarning($"{buttonName} button not assigned in the inspector!");
        }
    }

    void CheckAnimationComponent(Animation anim, string animName)
    {
        if (anim == null)
        {
            Debug.LogWarning($"{animName} Animation component not assigned in the inspector!");
        }
    }

    void SwitchToGameScene()
    {
        SceneManager.LoadScene("game");
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void ToggleSettingsMenu()
    {
        isSettingsOpen = !isSettingsOpen;

        if (isSettingsOpen)
        {
            PlayAnimation(settingsAnimation, "OpenSettings");
            PlayAnimation(benchAnimation, "Bench_drop");
        }
        else
        {
            // Add logic to close the settings menu if needed
            // For example:
            // PlayAnimation(settingsAnimation, "settingsDown");
            // PlayAnimation(benchAnimation, "Bench_rise");
        }
    }

    void PlayAnimation(Animation anim, string clipName)
    {
        if (anim != null && anim.GetClip(clipName) != null)
        {
            anim[clipName].wrapMode = WrapMode.Once;
            anim.Play(clipName);
        }
        else
        {
            Debug.LogWarning($"Unable to play animation: {clipName}. Animation or clip is missing.");
        }
    }
}