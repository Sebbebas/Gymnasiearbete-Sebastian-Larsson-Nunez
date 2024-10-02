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

    //Direction enums
    [SerializeField] private MovementDirection directionOfInput;
    [SerializeField] private MovementDirection lastPreformed;
    [SerializeField] private MovementDirection preFireInputDirection;

    //Direction Vector
    private Vector2 latestPreformedMoveDirection;
    private Vector2 currentPreFireDirection;

    private Vector2 currentMoveDirection;

    // Cached References \\
    InputAction movementAction;
    Rigidbody2D myrigidbody;
    BoxCollider2D playerCollider;

    private void Start()
    {
        //Get Cached References
        myrigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    private void FixedUpdate()
    {
        //can move?
        if(moveReset == true)
        {
            //Move in the same direction as the last "Prefire Input"
            if (preFireInputDirection != MovementDirection.none)
            {
                currentMoveDirection = currentPreFireDirection;
                ResetDirections();
            }

            //Move in the same direction as the last "Input"
            else if (directionOfInput != MovementDirection.none)
            {
                currentMoveDirection = latestPreformedMoveDirection;
                ResetDirections();
            }
        }

        //Move the player in the current direction 
        myrigidbody.linearVelocity = currentMoveDirection * movementSpeed;

        //Reset move when OverlapCircle collides with wall
        if (Physics2D.OverlapCircle((Vector2)transform.position + (currentMoveDirection * gizmoOffset), gizmoRadius, gizmoLayerMask)) { moveReset = true; }

        //reduce the currentPreFireInputTime with Time.deltaTime if its greater then 0
        if (currentPreFireInputTime > 0) 
        { 
            currentPreFireInputTime -= Time.deltaTime;

            //When the currentPreFireInputTime reaches 0 reset the currentPreFireDirection
            if (currentPreFireInputTime <= 0) 
            { 
                currentPreFireInputTime = 0; currentPreFireDirection = Vector2.zero; preFireInputDirection = MovementDirection.none; 
            } 
        }
    }

    private void ResetDirections()
    {
        //Reset PreFire Direction
        preFireInputDirection = MovementDirection.none;
        currentPreFireDirection = Vector2.zero;

        //Reset Last Direction
        directionOfInput = MovementDirection.none;
        latestPreformedMoveDirection = Vector2.zero;

        //Cant change move direction 
        moveReset = false;
    }

    #region Get Inputs
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
            PreFire(direction);
            return;
        }
        //Else <--------------- CONTINUE HERE 
        else
        {
            if (direction.x != 0 && latestPreformedMoveDirection.x != direction.x) { this.directionOfInput = (direction.x > 0) ? MovementDirection.right : MovementDirection.left; }
            if (direction.y != 0 && latestPreformedMoveDirection.y != direction.y) { this.directionOfInput = (direction.y > 0) ? MovementDirection.up : MovementDirection.down; }
            latestPreformedMoveDirection = direction;
        }
    }
    public void PreFire(Vector3 direction)
    {
        if (direction.x != 0 && currentPreFireDirection.x != direction.x) { preFireInputDirection = (direction.x > 0) ? MovementDirection.right : MovementDirection.left; }
        if (direction.y != 0 && currentPreFireDirection.y != direction.y) { preFireInputDirection = (direction.y > 0) ? MovementDirection.up : MovementDirection.down; }
        currentPreFireDirection = direction;

        currentPreFireInputTime = preFireMoveTime;
    }

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
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + (currentMoveDirection * gizmoOffset), gizmoRadius);
    }
}
