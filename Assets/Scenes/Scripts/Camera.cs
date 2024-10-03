using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraFollowSpeed = 2f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 15f;
    public float orthographicSizeMargin = 2f;
    public float cameraHeight = 10f;

    private Vector3 lastMidpoint;

    // Colliders for camera borders
    private BoxCollider2D leftBorder;
    private BoxCollider2D rightBorder;

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
        UpdateCameraPosition(Vector3.zero, Vector3.zero, true);
        lastMidpoint = mainCamera.transform.position;

        // Create the border colliders
        CreateBorders();
        UpdateBorders();
    }

    // Update the camera's position based on the players' positions
    public void UpdateCameraPosition(Vector3 player1Pos, Vector3 player2Pos, bool immediate = false)
    {
        if (mainCamera == null) return;

        // Midpoint on the X-axis, maintaining a fixed Z position for the camera
        Vector3 midpoint = new Vector3(
            (player1Pos.x + player2Pos.x) / 2f, // X position based on players' midpoint
            cameraHeight,                       // Y position fixed to camera height
            mainCamera.transform.position.z     // Z axis locked to the current camera Z position
        );

        // Calculate the distance between players using only the X axis
        float distanceBetweenPlayersX = Mathf.Abs(player1Pos.x - player2Pos.x);

        // Adjust orthographic size based on player X-axis distance only
        float requiredOrthoSize = (distanceBetweenPlayersX / 2f) + orthographicSizeMargin;
        float newOrthoSize = Mathf.Clamp(requiredOrthoSize, minOrthoSize, maxOrthoSize);

        if (immediate)
        {
            mainCamera.transform.position = midpoint; // Move immediately to new midpoint
            mainCamera.orthographicSize = newOrthoSize;
            lastMidpoint = midpoint;
        }
        else
        {
            // Smoothly move towards the new midpoint on the X axis, and lock the Z axis
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, midpoint, Time.deltaTime * cameraFollowSpeed);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed);

            // Smoothen transitions if the midpoint has shifted significantly
            if (Vector2.Distance(new Vector2(midpoint.x, midpoint.z), new Vector2(lastMidpoint.x, lastMidpoint.z)) > 0.1f)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed * 2f);
            }

            lastMidpoint = midpoint;
        }

        UpdateBorders();
    }

    // Create the borders around the camera's edges
    private void CreateBorders()
    {
        leftBorder = new GameObject("Left Border").AddComponent<BoxCollider2D>();
        rightBorder = new GameObject("Right Border").AddComponent<BoxCollider2D>();

        // Set borders to be triggers (optional, based on gameplay)
        leftBorder.isTrigger = false;
        rightBorder.isTrigger = false;
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

        // Update the positions and sizes of the borders to match the camera's vertical view size
        leftBorder.transform.position = new Vector3(cameraPos.x - halfWidth - 0.5f, cameraPos.y, 5f); // Set Z to 5
        leftBorder.size = new Vector3(1, halfHeight * 2, 1);  // Adjust Y size to match the camera's height

        rightBorder.transform.position = new Vector3(cameraPos.x + halfWidth + 0.5f, cameraPos.y, 5f); // Set Z to 5
        rightBorder.size = new Vector3(1, halfHeight * 2, 1);  // Adjust Y size to match the camera's height
    }
}