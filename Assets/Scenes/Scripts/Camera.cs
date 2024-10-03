using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraFollowSpeed = 2f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 15f;
    public float orthographicSizeMargin = 2f;

    public GameObject player1;
    public GameObject player2;

    private Vector3 lastMidpoint;

    // Colliders for camera borders
    private BoxCollider2D leftBorder;
    private BoxCollider2D rightBorder;
    private BoxCollider2D topBorder;
    private BoxCollider2D bottomBorder;

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
        UpdateCameraPosition(true);
        lastMidpoint = mainCamera.transform.position;

        // Create the border colliders
        CreateBorders();
        UpdateBorders();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    // Update the camera's position based on the players' positions
    public void UpdateCameraPosition(bool immediate = false)
    {
        if (player1 == null || player2 == null || mainCamera == null) return;

        Vector3 player1Pos = player1.transform.position;
        Vector3 player2Pos = player2.transform.position;

        // Calculate the center point between the two players
        Vector3 centerPoint = (player1Pos + player2Pos) / 2f;
        centerPoint.z = mainCamera.transform.position.z; // Maintain camera's z-position

        // Calculate the required orthographic size to fit both players
        float distanceX = Mathf.Abs(player1Pos.x - player2Pos.x) / 2f;
        float distanceY = Mathf.Abs(player1Pos.y - player2Pos.y) / 2f;
        
        float aspectRatio = Screen.width / (float)Screen.height;
        float orthographicSizeX = distanceX / aspectRatio;
        float orthographicSizeY = distanceY;

        // Use the larger of the two sizes to ensure both players are in view
        float newOrthoSize = Mathf.Max(orthographicSizeX, orthographicSizeY) + orthographicSizeMargin;
        newOrthoSize = Mathf.Clamp(newOrthoSize, minOrthoSize, maxOrthoSize);

        if (immediate)
        {
            mainCamera.transform.position = centerPoint;
            mainCamera.orthographicSize = newOrthoSize;
            lastMidpoint = centerPoint;
        }
        else
        {
            // Smoothly move towards the new position and ortho size
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, centerPoint, Time.deltaTime * cameraFollowSpeed);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed);

            lastMidpoint = centerPoint;
        }

        UpdateBorders();
    }

    // Create the borders around the camera's edges
    private void CreateBorders()
    {
        leftBorder = new GameObject("Left Border").AddComponent<BoxCollider2D>();
        rightBorder = new GameObject("Right Border").AddComponent<BoxCollider2D>();
        topBorder = new GameObject("Top Border").AddComponent<BoxCollider2D>();
        bottomBorder = new GameObject("Bottom Border").AddComponent<BoxCollider2D>();

        // Set borders to be triggers (optional, based on gameplay)
        leftBorder.isTrigger = rightBorder.isTrigger = topBorder.isTrigger = bottomBorder.isTrigger = false;
    }

    // Update the borders' positions and sizes based on the camera's orthographic size
    private void UpdateBorders()
    {
        float cameraOrthoSize = mainCamera.orthographicSize;
        float cameraAspect = mainCamera.aspect;

        // Calculate the half-height and half-width of the camera's view
        float halfHeight = cameraOrthoSize;
        float halfWidth = halfHeight * cameraAspect;

        Vector3 cameraPos = mainCamera.transform.position;

        // Update the positions and sizes of all borders
        leftBorder.transform.position = new Vector3(cameraPos.x - halfWidth - 0.5f, cameraPos.y, 5f);
        leftBorder.size = new Vector3(1, halfHeight * 2, 1);

        rightBorder.transform.position = new Vector3(cameraPos.x + halfWidth + 0.5f, cameraPos.y, 5f);
        rightBorder.size = new Vector3(1, halfHeight * 2, 1);

        topBorder.transform.position = new Vector3(cameraPos.x, cameraPos.y + halfHeight + 0.5f, 5f);
        topBorder.size = new Vector3(halfWidth * 2, 1, 1);

        bottomBorder.transform.position = new Vector3(cameraPos.x, cameraPos.y - halfHeight - 0.5f, 5f);
        bottomBorder.size = new Vector3(halfWidth * 2, 1, 1);
    }
}