using UnityEngine;
using UnityEngine.InputSystem;

public enum MovementDirection
{
    none, up, down, left, right
}

public class PlayerMovement : MonoBehaviour
{
    // Configurable Parameters \\
    [Header("Input")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Movement")]
    [SerializeField, Tooltip("Sebbe va gör movementSpeed? -Viggo")] float movementSpeed = 1.0f;
    [SerializeField, Tooltip("The time it takes for a new Input to be canceld")] float preFireMoveTime = 1.0f;

    [Header("Gizmos")]
    [SerializeField] LayerMask gizmoLayerMask;
    [SerializeField] float gizmoOffset;
    [SerializeField] float gizmoRadius;

    // Private Variables \\
    private float currentPreFireInputTime;
    private bool moveReset = true;

    //Current Direction
    private Vector2 currentMoveDirection;

    //Current pressed Input
    private MovementDirection directionOfInput;
    private Vector2 directionOfInputVector2; 

    //Last performed Input
    private MovementDirection lastPreformedInput;
    private Vector2 lastPreformedInputVector2;

    //Current "preFireInput"
    private MovementDirection preFireDirection;
    private Vector2 preFireDirectionVector2;

    // Cached References \\
    InputAction movementAction;
    Rigidbody2D myrigidbody;
    BoxCollider2D[] playerCollider;

    #region Base Methods
    private void Start()
    {
        //Get Cached References
        myrigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponents<BoxCollider2D>();
    }
    private void FixedUpdate()
    {
        //can move?
        if (moveReset == true)
        {
            //Enable Colliders
            playerCollider[0].enabled = true;
            playerCollider[1].enabled = true;

            //Move in the same direction as the last "Prefire Input"
            if (preFireDirection != MovementDirection.none)
            {
                //Set "currentMoveDirection" to "preFireDirection"
                currentMoveDirection = Vector2.zero;
                currentMoveDirection = GetDirectionVector(preFireDirection);

                //Update last performed
                lastPreformedInput = preFireDirection;
                ResetDirections();
            }

            //Move in the same direction as the last "Input"
            else if (directionOfInput != MovementDirection.none)
            {
                //Set "currentMoveDirection" to "directionOfInput"
                currentMoveDirection = Vector2.zero;
                currentMoveDirection = GetDirectionVector(directionOfInput);

                lastPreformedInput = directionOfInput;
                ResetDirections();
            }

            //Move in the "lastPreformedInput" Direction
            else if (lastPreformedInputVector2 != GetDirectionVector(lastPreformedInput))
            {
                lastPreformedInputVector2 = Vector2.zero;
                lastPreformedInputVector2 = GetDirectionVector(lastPreformedInput);
            }
        }

        //Disable vertical collider
        else if(currentMoveDirection.x != 0)
        {
            //Change size
            playerCollider[0].enabled = true;
            playerCollider[1].enabled = false;
        }

        //Disable horizontal collider
        else if (currentMoveDirection.y != 0)
        {
            //Change size
            playerCollider[1].enabled = true;
            playerCollider[0].enabled = false;
        }

        //Move the player in the current direction 
        myrigidbody.linearVelocity = currentMoveDirection * movementSpeed;

        //Reset move when OverlapCircle collides with wall
        if (Physics2D.OverlapCircle((Vector2)transform.position + (currentMoveDirection * gizmoOffset), gizmoRadius, gizmoLayerMask)) { moveReset = true; }

        //reduce the currentPreFireInputTime with Time.deltaTime if its greater then 0
        if (currentPreFireInputTime > 0) 
        { 
            currentPreFireInputTime -= Time.deltaTime;

            //When the currentPreFireInputTime reaches 0 reset the preFireDirectionVector2
            if (currentPreFireInputTime <= 0) 
            { 
                currentPreFireInputTime = 0; preFireDirectionVector2 = Vector2.zero; preFireDirection = MovementDirection.none; 
            } 
        }
    }
    private void OnEnable()
    {
        //Create a actionMap and Find "Player"
        var actionMap = inputActions.FindActionMap("Player");

        //Find "Move" action in "Player"
        movementAction = actionMap.FindAction("Move");

        //Enable Action
        movementAction.Enable();

        //Subscribe to the performed action
        movementAction.performed += OnMove;
    }
    private void OnDisable()
    {
        //Unsubscribe to the performed action
        movementAction.performed -= OnMove;
    }
    #endregion

    #region Movement Direction
    /// <summary>
    /// When OnMove is called Get the <paramref name="context"/> of the input and do stuff
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        //Get Input direction Context in Vector2 form
        Vector2 direction = context.ReadValue<Vector2>();

        //Round the Input direction to closest Int (From ±0.72 to 1)
        direction.x = Mathf.RoundToInt(direction.x);
        direction.y = Mathf.RoundToInt(direction.y);

        //If the player dose not touch a wall set a new Prefire direction
        if (moveReset == false)
        {
            SetPreFireDirection(direction);
            return;
        }

        //Else set the "directionOfInputVector2" based on "lastPreformedInput"
        else
        {
            SetMoveDirection(direction);
            lastPreformedInputVector2 = direction;
            return;
        }
    }
    private void SetMoveDirection(Vector2 direction)
    {
        //Based on the "lastPreformedInput" update directionOfInput
        if (direction.x != 0 && lastPreformedInputVector2.x != direction.x) { directionOfInput = (direction.x > 0) ? MovementDirection.right : MovementDirection.left; }
        else if (direction.y != 0 && lastPreformedInputVector2.y != direction.y) { directionOfInput = (direction.y > 0) ? MovementDirection.up : MovementDirection.down; }

        //Update last performed
        if (directionOfInput != MovementDirection.none) { lastPreformedInput = directionOfInput; directionOfInputVector2 = direction; }
    }

    private void SetPreFireDirection(Vector2 direction)
    {
        //Based on the "lastPreformedInput" update "preFireDirection"
        if (direction.x != 0 && preFireDirectionVector2.x != direction.x) { preFireDirection = (direction.x > 0) ? MovementDirection.right : MovementDirection.left; }
        else if(direction.y != 0 && preFireDirectionVector2.y != direction.y) { preFireDirection = (direction.y > 0) ? MovementDirection.up : MovementDirection.down; }

        //Set the time before the prefire expires
        if (preFireDirection != MovementDirection.none) { currentPreFireInputTime = preFireMoveTime; preFireDirectionVector2 = direction; }
    }
    private void ResetDirections()
    {
        //Reset SetPreFireDirection Direction
        preFireDirection = MovementDirection.none;
        preFireDirectionVector2 = Vector2.zero;

        //Reset Last Direction
        directionOfInput = MovementDirection.none;
        directionOfInputVector2 = Vector2.zero;

        //Cant change move direction 
        moveReset = false;
    }
    /// <summary>
    /// Get a Vector2 using <paramref name="direction"/>
    /// </summary>
    private Vector2 GetDirectionVector(MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.up:
                return Vector2.up;
            case MovementDirection.right:
                return Vector2.right;
            case MovementDirection.down:
                return Vector2.down;
            case MovementDirection.left:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }
    #endregion

    public void OnDrawGizmosSelected()
    {
        //Wall Check
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + (currentMoveDirection * gizmoOffset), gizmoRadius);

        //Hit Collider
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
    public Vector2 GetCurrentMoveDirecion()
    {
        return currentMoveDirection;
    }
}
