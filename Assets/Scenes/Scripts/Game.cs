using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MyPlayerScript : MonoBehaviour
{
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log(context.performed);
    }
}