using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum MovementDirection
{
    none, up, down, left, right
}

public class PlayerMovement : MonoBehaviour
{
    //Configurable Parameters
    [Header("Input")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] MovementDirection lastDirection;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 1.0f;

    //Private Variables
    private Vector3 lastInput;

    //Cached References
    InputAction movementAction;
    Rigidbody2D myrigidbody;
    BoxCollider2D playerCollider;

    private void Start()
    {
        myrigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        {
            //Rotate the player based on the last direction
            float rotationAngle = 0f;
            Vector2 moveDirection = Vector2.zero;

            //Determine the angle and the movement direction
            switch (lastDirection)
            {
                case MovementDirection.up:
                    rotationAngle = 0f;
                    moveDirection = Vector2.up;
                    break;
                case MovementDirection.right:
                    rotationAngle = -90f;
                    moveDirection = Vector2.right;
                    break;
                case MovementDirection.down:
                    rotationAngle = 180f;
                    moveDirection = Vector2.down;
                    break;
                case MovementDirection.left:
                    rotationAngle = 90f;
                    moveDirection = Vector2.left;
                    break;
            }

            //Add rotation
            myrigidbody.MoveRotation(rotationAngle);

            //Move forward in the direction the player is facing
            myrigidbody.linearVelocity = moveDirection * movementSpeed;
        }
    }
    private void OnEnable()
    {
        //Get the action map
        var actionMap = inputActions.FindActionMap("Player");
        movementAction = actionMap.FindAction("Move");

        //Enable the action
        movementAction.Enable();

        //Subscribe to the preformed callback
        movementAction.performed += OnMove;
    }
    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        movementAction.performed -= OnMove;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //Get Input Direction
        Vector2 direction = context.ReadValue<Vector2>();

        //Round Input Direction to neareast int
        direction.x = Mathf.RoundToInt(direction.x);
        direction.y = Mathf.RoundToInt(direction.y);

        //Get the latest input 
        if(direction.x != 0 && lastInput.x != direction.x) { if (direction.x > 0) { lastDirection = MovementDirection.right; } else lastDirection = MovementDirection.left; }
        if(direction.y != 0 && lastInput.y != direction.y) { if (direction.y > 0) { lastDirection = MovementDirection.up; }  else lastDirection = MovementDirection.down; }

        //Set new lastInput
        lastInput = direction;
    }
}
