using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Vector2 playerMovement;
    public Rigidbody2D rb;
    public bool isGrounded;
    private bool player1;
    public LayerMask groundLayer;
    public float rayLength;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
    }
    
    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            playerMovement = context.ReadValue<Vector2>();
            Debug.Log("Player 1 Input: " + playerMovement);
        }

        // Trigger jump if "W" is pressed
        if (context.performed && playerMovement.y > 0 && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    { 
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);  // Apply force upward for jump
    }
    
    private void CheckGrounded()
    { // Starting point of the ray (usually your object's position)
        Vector3 rayOrigin = transform.position;
        
        // Direction of the ray (here we're using forward direction)
        Vector3 rayDirection = Vector2.down;
        

        // Cast the ray
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);


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
