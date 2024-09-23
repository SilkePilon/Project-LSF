using UnityEngine;
using UnityEngine.InputSystem;

public class SimplifiedTwoPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f; // Jump height
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

    private Rigidbody player1Rb;
    private Rigidbody player2Rb;
    private bool isPlayer1Grounded;
    private bool isPlayer2Grounded;

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

        mainCamera.orthographic = true;
        UpdateCameraPosition(true);
        lastMidpoint = mainCamera.transform.position;

        player1Rb = player1.GetComponent<Rigidbody>();
        player2Rb = player2.GetComponent<Rigidbody>();
    }

    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            player1Movement = context.ReadValue<Vector2>();
            Debug.Log("Player 1 Input: " + player1Movement);
        }
    }

    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            player2Movement = context.ReadValue<Vector2>();
            Debug.Log("Player 2 Input: " + player2Movement);
        }
    }

    public void OnPlayer1Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isPlayer1Grounded)
        {
            Jump(player1Rb, player1Movement);
        }
    }

    public void OnPlayer2Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isPlayer2Grounded)
        {
            Jump(player2Rb, player2Movement);
        }
    }

    private void Update()
    {
        if (player1 != null && player2 != null)
        {
            MovePlayer(player1, player1Movement);
            MovePlayer(player2, player2Movement);
            UpdateCameraPosition();
            CheckGrounded(); // Check if players are grounded
        }
    }

    private void MovePlayer(GameObject player, Vector2 movement)
    {
        if (player != null)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.y) * moveSpeed * Time.deltaTime;
            player.transform.Translate(move);
        }
    }

    private void Jump(Rigidbody playerRb, Vector2 movement)
    {
        if (playerRb != null)
        {
            Vector3 jumpDirection = new Vector3(movement.x, jumpForce, movement.y);
            playerRb.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }

    private void CheckGrounded()
    {
        // Use a raycast to check if the player is grounded
        isPlayer1Grounded = Physics.Raycast(player1.transform.position, Vector3.down, 1.1f);
        isPlayer2Grounded = Physics.Raycast(player2.transform.position, Vector3.down, 1.1f);
    }

    private void UpdateCameraPosition(bool immediate = false)
    {
        if (player1 == null || player2 == null || mainCamera == null) return;

        Vector3 midpoint = (player1.transform.position + player2.transform.position) / 2f;
        midpoint.y = cameraHeight;

        float distanceBetweenPlayers = Vector3.Distance(player1.transform.position, player2.transform.position);
        float requiredOrthoSize = (distanceBetweenPlayers / 2f) + orthographicSizeMargin;
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

            if (Vector3.Distance(midpoint, lastMidpoint) > 0.1f)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, newOrthoSize, Time.deltaTime * cameraFollowSpeed * 2f);
            }

            lastMidpoint = midpoint;
        }

        Debug.Log($"Camera Position: {mainCamera.transform.position}, Ortho Size: {mainCamera.orthographicSize}");
    }
}
