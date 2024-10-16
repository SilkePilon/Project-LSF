using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraFollowSpeed = 2f;
    public float minOrthoSize = 8f;
    public float maxOrthoSize = 12f;
    public float orthographicSizeMargin = 2f;
    public float cameraHeight = 2f;
    public GameObject player1;
    public GameObject player2;
    public GameObject pauseMenuCanvas; // Reference to the pause menu Canvas

    private bool isPaused = false;
    private Vector3 lastMidpoint;
    private float initialOrthoSize;
    private BoxCollider2D leftBorder;
    private BoxCollider2D rightBorder;
    private Vector3[] initialChildScales;
    private Vector3[] initialChildPositions;

    private void Start()
    {
        // Ensure the main camera is set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera == null)
        {
            Debug.LogError("No camera assigned and couldn't find main camera!");
            return;
        }

        mainCamera.orthographic = true;
        initialOrthoSize = mainCamera.orthographicSize;
        lastMidpoint = mainCamera.transform.position;

        CreateBorders();
        InitializeChildElements();

        // Ensure the pause menu canvas is hidden initially
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    private void InitializeChildElements()
    {
        initialChildScales = new Vector3[mainCamera.transform.childCount];
        initialChildPositions = new Vector3[mainCamera.transform.childCount];

        for (int i = 0; i < mainCamera.transform.childCount; i++)
        {
            Transform child = mainCamera.transform.GetChild(i);
            initialChildScales[i] = child.localScale;
            initialChildPositions[i] = child.localPosition;
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

    private void Update()
    {
        // Handle pause menu toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        // If not paused, continue updating the camera position and borders
        if (!isPaused)
        {
            UpdateCameraPosition(true);
            UpdateBorders();
            ScaleCameraChildren();
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

    // Update the camera's position based on the players' positions
    private void UpdateCameraPosition(bool immediate = false)
    {
        if (player1 == null || player2 == null || mainCamera == null) return;

        Vector3 player1Pos = player1.transform.position;
        Vector3 player2Pos = player2.transform.position;

        Vector3 midpoint = new Vector3(
            (player1Pos.x + player2Pos.x) / 2f,
            cameraHeight,
            mainCamera.transform.position.z
        );

        float distanceBetweenPlayersX = Mathf.Abs(player1Pos.x - player2Pos.x);
        float requiredOrthoSize = (distanceBetweenPlayersX / 2f) + orthographicSizeMargin;
        float newOrthoSize = Mathf.Clamp(requiredOrthoSize, minOrthoSize, maxOrthoSize);

        if (immediate)
        {
            mainCamera.transform.position = midpoint;
            mainCamera.orthographicSize = newOrthoSize;
            lastMidpoint = midpoint;
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, midpoint, Time.deltaTime * cameraFollowSpeed);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed);

            if (Vector2.Distance(new Vector2(midpoint.x, midpoint.z), new Vector2(lastMidpoint.x, lastMidpoint.z)) > 0.1f)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed * 2f);
            }

            lastMidpoint = midpoint;
        }
    }

    private void CreateBorders()
    {
        leftBorder = new GameObject("Left Border").AddComponent<BoxCollider2D>();
        rightBorder = new GameObject("Right Border").AddComponent<BoxCollider2D>();

        leftBorder.isTrigger = false;
        rightBorder.isTrigger = false;
    }

    private void UpdateBorders()
    {
        float cameraOrthoSize = mainCamera.orthographicSize;
        float cameraAspect = mainCamera.aspect;

        float halfHeight = cameraOrthoSize;
        float halfWidth = halfHeight * cameraAspect;

        Vector3 cameraPos = mainCamera.transform.position;

        leftBorder.transform.position = new Vector3(cameraPos.x - halfWidth - 0.5f, cameraPos.y, 5f);
        leftBorder.size = new Vector3(1, halfHeight * 2, 1);

        rightBorder.transform.position = new Vector3(cameraPos.x + halfWidth + 0.5f, cameraPos.y, 5f);
        rightBorder.size = new Vector3(1, halfHeight * 2, 1);
    }

    private void ScaleCameraChildren()
    {
        float scaleFactor = mainCamera.orthographicSize / initialOrthoSize;

        for (int i = 0; i < mainCamera.transform.childCount; i++)
        {
            Transform child = mainCamera.transform.GetChild(i);

            if (child.GetComponent<BoxCollider2D>() != null) continue;

            child.localScale = initialChildScales[i] / scaleFactor;
            child.localPosition = initialChildPositions[i] / scaleFactor;

            if (child.GetComponent<RectTransform>() != null)
            {
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                rectTransform.sizeDelta = rectTransform.sizeDelta * scaleFactor;
            }
        }
    }
    
}


