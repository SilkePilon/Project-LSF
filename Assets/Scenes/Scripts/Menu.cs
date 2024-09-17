using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraSwitch : MonoBehaviour
{
    
    public RectTransform settingsPanel;
    public RectTransform bench;

[Header("Settings")]
    [SerializeField]private bool isSettingsOpen = false;
    private Vector2 settingsClosedPosition;
    private Vector2 settingsOpenPosition;
    private Vector2 benchClosedPosition;
    private Vector2 benchOpenPosition;
    private float movementDuration = 1.5f;
    private float waitDuration = 0.3f;

    void Start()
    {
        

        InitializePositions();
    }

    void InitializePositions()
    {
        if (settingsPanel != null)
        {
            settingsClosedPosition = settingsPanel.anchoredPosition;
            settingsOpenPosition = new Vector2(settingsClosedPosition.x, settingsClosedPosition.y + 600f);
        }
        else
        {
            Debug.LogWarning("Settings panel not assigned in the inspector!");
        }

        if (bench != null)
        {
            benchClosedPosition = bench.anchoredPosition;
            benchOpenPosition = new Vector2(benchClosedPosition.x, benchClosedPosition.y - 200f);
        }
        else
        {
            Debug.LogWarning("Bench not assigned in the inspector!");
        }
    }

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleSettingsMenu()
    {
        isSettingsOpen = !isSettingsOpen;
        StartCoroutine(MoveElementsSequentially());
    }

    IEnumerator MoveElementsSequentially()
    {
        if (isSettingsOpen)
        {
            // Move bench down
            yield return StartCoroutine(MoveElement(bench, benchClosedPosition, benchOpenPosition));

            // Wait for a second
            yield return new WaitForSeconds(waitDuration);

            // Move settings panel up
            yield return StartCoroutine(MoveElement(settingsPanel, settingsClosedPosition, settingsOpenPosition));
        }
        else
        {
            // Move settings panel down
            yield return StartCoroutine(MoveElement(settingsPanel, settingsOpenPosition, settingsClosedPosition));

            // Wait for a second
            yield return new WaitForSeconds(waitDuration);

            // Move bench up
            yield return StartCoroutine(MoveElement(bench, benchOpenPosition, benchClosedPosition));
        }
    }

    IEnumerator MoveElement(RectTransform element, Vector2 startPos, Vector2 endPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / movementDuration;
            element.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        element.anchoredPosition = endPos; // Ensure the element reaches its final position
    }
}