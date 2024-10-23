using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    private Vector2 playerMovement;
    public Rigidbody2D rb;
    public Collider2D col;
    public Collider2D platformCol;
    public bool isGrounded;
    private bool player1;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    public float rayLength;
    private RaycastHit2D hit;
    public bool isDropping;
    private PlayerInput pInput;
    public InputActionAsset p1;
    public InputActionAsset p2;
    public bool isPlayer1;

    // Start is called before the first frame update
    void Start()
    {
        pInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        if (isPlayer1)
        {
            pInput.actions = p1;
        }
        else
        {
            pInput.actions = p2;
        }

    }
    // Update is called once per frame
    private void Update()
    {
        if (rb.velocity.y == 0 && isDropping)
        {
            isDropping = false;
            Physics2D.IgnoreCollision(col, platformCol, false);
        }
        MovePlayer(playerMovement);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            playerMovement = context.ReadValue<Vector2>();
            Debug.Log("Player  Input: " + playerMovement);
        }

         if (playerMovement.x != 0)
        {
            // Invert the x scale to flip the character
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(playerMovement.x);
            transform.localScale = newScale;
        }

        // Trigger jump if "W" is pressed
        if (context.performed && playerMovement.y > 0 && isGrounded && rb.velocity.y <= 0)
        {
            Jump();
        }
        if (context.performed && playerMovement.y < 0 && isGrounded)
        {
            DropPlayer();
        }
        
    }
    
    private void MovePlayer(Vector2 movement)
    {
        {
            Vector3 move = new Vector3(-movement.x, 0, 0) * moveSpeed * Time.deltaTime;  // Only horizontal movement
            transform.Translate(move);
        }
    }

    private void Jump()
    { 
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);  // Apply force upward for jump
    }
    
    private void DropPlayer()
    {
        if (hit.collider !=null && hit.collider.CompareTag("Platform"))
        {
            isDropping = true;
            platformCol = hit.collider;
            Physics2D.IgnoreCollision(col, platformCol);
            rb.AddForce(-Vector3.up * 0.1f, ForceMode2D.Impulse);  //voeg weer toe als die vaag doet met droppen
            
        }
        
        // Visualize the ray in the Scene view
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.green);
    }

    private void CheckGrounded()
    { // Starting point of the ray (usually your object's position)
        // Cast the ray
        hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);


        if (hit.collider !=null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
        // Visualize the ray in the Scene view
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);

    }

}
