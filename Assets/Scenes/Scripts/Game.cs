using UnityEngine;
using UnityEngine.InputSystem;

public class TwoPlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInputAsset;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;  // Force applied for jumping

    // Player 1
    public Transform player1;
    private Rigidbody2D player1Rigidbody;
    private InputAction player1MoveAction;
    private InputAction player1JumpAction;
    private Vector2 player1MovementInput;
    private bool isPlayer1Grounded = true;

    // Player 2
    public Transform player2;
    private Rigidbody2D player2Rigidbody;
    private InputAction player2MoveAction;
    private InputAction player2JumpAction;
    private Vector2 player2MovementInput;
    private bool isPlayer2Grounded = true;

    private void OnEnable()
    {
        // Player 1 controls
        var player1Controls = playerInputAsset.FindActionMap("Player1Controls");
        player1MoveAction = player1Controls.FindAction("Move");
        player1JumpAction = player1Controls.FindAction("Jump");

        player1MoveAction.Enable();
        player1JumpAction.Enable();

        player1MoveAction.performed += ctx => player1MovementInput = ctx.ReadValue<Vector2>();
        player1MoveAction.canceled += ctx => player1MovementInput = Vector2.zero;

        player1JumpAction.performed += Player1Jump;

        // Player 2 controls
        var player2Controls = playerInputAsset.FindActionMap("Player2Controls");
        player2MoveAction = player2Controls.FindAction("Move");
        player2JumpAction = player2Controls.FindAction("Jump");

        player2MoveAction.Enable();
        player2JumpAction.Enable();

        player2MoveAction.performed += ctx => player2MovementInput = ctx.ReadValue<Vector2>();
        player2MoveAction.canceled += ctx => player2MovementInput = Vector2.zero;

        player2JumpAction.performed += Player2Jump;

        Debug.Log("Two players' controls enabled.");
    }

    private void OnDisable()
    {
        player1MoveAction.Disable();
        player1JumpAction.Disable();
        player2MoveAction.Disable();
        player2JumpAction.Disable();
    }

    private void Start()
    {
        // Assign Rigidbody2D for Player 1 and Player 2
        if (player1 != null)
        {
            player1Rigidbody = player1.GetComponent<Rigidbody2D>();
            player1Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;  // Prevent rotation in 2D
        }

        if (player2 != null)
        {
            player2Rigidbody = player2.GetComponent<Rigidbody2D>();
            player2Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;  // Prevent rotation in 2D
        }
    }

    private void Player1Jump(InputAction.CallbackContext context)
    {
        if (isPlayer1Grounded && player1Rigidbody != null)
        {
            player1Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isPlayer1Grounded = false;
            Debug.Log("Player 1 Jumped");
        }
    }

    private void Player2Jump(InputAction.CallbackContext context)
    {
        if (isPlayer2Grounded && player2Rigidbody != null)
        {
            player2Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isPlayer2Grounded = false;
            Debug.Log("Player 2 Jumped");
        }
    }

    private void Update()
    {
        // Player 1 movement
        if (player1 != null)
        {
            Vector2 player1Movement = new Vector2(player1MovementInput.x * moveSpeed, player1Rigidbody.velocity.y);
            player1Rigidbody.velocity = new Vector2(player1Movement.x, player1Rigidbody.velocity.y); // Preserve vertical velocity
        }

        // Player 2 movement
        if (player2 != null)
        {
            Vector2 player2Movement = new Vector2(player2MovementInput.x * moveSpeed, player2Rigidbody.velocity.y);
            player2Rigidbody.velocity = new Vector2(player2Movement.x, player2Rigidbody.velocity.y); // Preserve vertical velocity
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Check for both players' collisions with ground
            if (collision.collider.gameObject == player1.gameObject)
            {
                isPlayer1Grounded = true;
                Debug.Log("Player 1 grounded");
            }

            if (collision.collider.gameObject == player2.gameObject)
            {
                isPlayer2Grounded = true;
                Debug.Log("Player 2 grounded");
            }
        }
    }
}
