using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimplifiedTwoPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
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

    public Rigidbody2D player1Rb;
    private Rigidbody2D player2Rb;
    public bool isPlayer1Grounded;
    private bool isPlayer2Grounded;

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
        UpdateCameraPosition(true);
        lastMidpoint = mainCamera.transform.position;

        // Get the Rigidbody components for both players
        player1Rb = player1.GetComponent<Rigidbody2D>();
        player2Rb = player2.GetComponent<Rigidbody2D>();

        // Create the border colliders
        CreateBorders();
        UpdateBorders();
    }

    // Player 1 Movement Input
    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            player1Movement = context.ReadValue<Vector2>();
            Debug.Log("Player 1 Input: " + player1Movement);
        }

        // Trigger jump if "W" is pressed
        if (context.performed && player1Movement.y > 0 /*&& isPlayer1Grounded*/)
        {
            Jump(player1Rb);
        }
    }

    // Player 2 Movement Input
    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            player2Movement = context.ReadValue<Vector2>();
            Debug.Log("Player 2 Input: " + player2Movement);
        }

        // Trigger jump if "Arrow Up" is pressed
        if (context.performed && player2Movement.y > 0 && isPlayer2Grounded)
        {
            Jump(player2Rb);
        }
    }
    
    private void Update()
    {
        // Update player movement and camera position
        if (player1 != null && player2 != null)
        {
            MovePlayer(player1, player1Movement);
            MovePlayer(player2, player2Movement);
            UpdateCameraPosition();
            UpdateBorders(); // Update borders when the camera moves
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    // Move the player using the movement input
    private void MovePlayer(GameObject player, Vector2 movement)
    {
        if (player != null)
        {
            Vector3 move = new Vector3(-movement.x, 0, 0) * moveSpeed * Time.deltaTime;  // Only horizontal movement
            player.transform.Translate(move);
        }
    }

    // Jump with the specified Rigidbody (for either player)
    private void Jump(Rigidbody2D playerRb)
    {
        if (playerRb != null)
        {
            Debug.Log("weeeeeeeeeeee");
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);  // Apply force upward for jump
            Debug.Log($"Player jumped with force: {jumpForce}");
        }
    }

    // Check if the players are grounded using a Raycast
    private void CheckGrounded()
    { // Starting point of the ray (usually your object's position)
        Vector3 rayOrigin = player1.transform.position;
        
        // Direction of the ray (here we're using forward direction)
        Vector3 rayDirection = -player2.transform.up;
        
        // Length of the ray
        float rayLength = 100f;

        // Cast the ray
        Ray ray = new Ray(rayOrigin, Vector2.down);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, rayLength))
        {
            Debug.Log(hit.collider.name);
        }
        else
        {
            Debug.Log("ik collide niet");
        }
      //  isPlayer1Grounded = 
       
            
            
            isPlayer2Grounded = Physics.Raycast(player2.transform.position, Vector3.down, 1.1f);
    
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);
    }

    // Update the camera's position based on the players' positions
    private void UpdateCameraPosition(bool immediate = false)
    {
        if (player1 == null || player2 == null || mainCamera == null) return;

        // Calculate midpoint using only the X and Z coordinates of players
        Vector3 player1Pos = player1.transform.position;
        Vector3 player2Pos = player2.transform.position;

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

        //Debug.Log($"Camera Position: {mainCamera.transform.position}, Ortho Size: {mainCamera.orthographicSize}");
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
