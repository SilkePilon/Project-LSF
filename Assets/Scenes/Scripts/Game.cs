using UnityEngine;
using UnityEngine.InputSystem;

public class SimplifiedTwoPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject player1;
    public GameObject player2;
    public Camera mainCamera;
    public float cameraFollowSpeed = 2f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 15f;
    public float orthographicSizeMargin = 2f;
    public float cameraHeight = 10f;

    private Vector2 player1Movement;
    private Vector2 player2Movement;
    private Vector3 lastMidpoint;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera == null)
        {
            Debug.LogError("No camera assigned and couldn't find main camera!");
            return;
        }
        
        // Ensure the camera is orthographic
        mainCamera.orthographic = true;
        
        // Initial camera position
        UpdateCameraPosition(true);
        lastMidpoint = mainCamera.transform.position;
    }

    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        player1Movement = context.ReadValue<Vector2>();
        Debug.Log("Player 1 Input: " + player1Movement);
    }

    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        player2Movement = context.ReadValue<Vector2>();
        Debug.Log("Player 2 Input: " + player2Movement);
    }

    private void Update()
    {
        MovePlayer(player1, player1Movement);
        MovePlayer(player2, player2Movement);
        UpdateCameraPosition();
    }

    private void MovePlayer(GameObject player, Vector2 movement)
    {
        if (player != null)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.y) * moveSpeed * Time.deltaTime;
            player.transform.Translate(move);
        }
    }

    private void UpdateCameraPosition(bool immediate = false)
    {
        if (player1 == null || player2 == null || mainCamera == null) return;

        // Calculate the midpoint between the two players
        Vector3 midpoint = (player1.transform.position + player2.transform.position) / 2f;
        midpoint.y = cameraHeight;

        // Calculate the distance between players
        float distanceBetweenPlayers = Vector3.Distance(player1.transform.position, player2.transform.position);

        // Calculate the required orthographic size to fit both players
        float requiredOrthoSize = (distanceBetweenPlayers / 2f) + orthographicSizeMargin;

        // Clamp the orthographic size between min and max values
        float newOrthoSize = Mathf.Clamp(requiredOrthoSize, minOrthoSize, maxOrthoSize);

        if (immediate)
        {
            mainCamera.transform.position = midpoint;
            mainCamera.orthographicSize = newOrthoSize;
            lastMidpoint = midpoint;
        }
        else
        {
            // Smoothly move the camera towards the midpoint
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, midpoint, Time.deltaTime * cameraFollowSpeed);
            
            // Smoothly adjust the camera's orthographic size
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed);

            // If players are moving apart, zoom out faster
            if (Vector3.Distance(midpoint, lastMidpoint) > 0.1f)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed * 2f);
            }

            lastMidpoint = midpoint;
        }

        Debug.Log($"Camera Position: {mainCamera.transform.position}, Ortho Size: {mainCamera.orthographicSize}");
    }
}