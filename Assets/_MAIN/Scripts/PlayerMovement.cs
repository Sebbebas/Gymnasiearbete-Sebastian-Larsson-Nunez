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

    //Direction
    private MovementDirection lastDirection;
    private MovementDirection preFireInputDirection;

    private Vector2 latestPreformedMoveDirection;
    private Vector2 currentPreFireDirection;
    private Vector2 currentMoveDirection;

    private float currentPreFireInputTime;
    private bool moveReset = true;

    // Cached References \\
    InputAction movementAction;
    Rigidbody2D myrigidbody;
    BoxCollider2D playerCollider;

    private void Start()
    {
        myrigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    private void FixedUpdate()
    {
        if(moveReset == true)
        {
            if (preFireInputDirection != MovementDirection.none)
            {
                currentMoveDirection = currentPreFireDirection;
                ResetDirections();
            }
            else if (lastDirection != MovementDirection.none)
            {
                currentMoveDirection = latestPreformedMoveDirection;
                ResetDirections();
            }
        }

        myrigidbody.linearVelocity = currentMoveDirection * movementSpeed;

        //Input
        if (Physics2D.OverlapCircle((Vector2)transform.position + (currentMoveDirection * gizmoOffset), gizmoRadius, gizmoLayerMask)) { moveReset = true; }
        if (currentPreFireInputTime > 0) { currentPreFireInputTime -= Time.deltaTime; if (currentPreFireInputTime <= 0) { currentPreFireInputTime = 0; currentPreFireDirection = Vector2.zero; preFireInputDirection = MovementDirection.none; } }
    }

    private void ResetDirections()
    {
        //Reset PreFire Direction
        preFireInputDirection = MovementDirection.none;
        currentPreFireDirection = Vector2.zero;

        //Reset Last Direction
        lastDirection = MovementDirection.none;
        latestPreformedMoveDirection = Vector2.zero;

        //Cant change Direction Again
        moveReset = false;
    }

    #region Get Inputs
    private void OnEnable()
    {
        var actionMap = inputActions.FindActionMap("Player");
        movementAction = actionMap.FindAction("Move");
        movementAction.Enable();
        movementAction.performed += OnMove;
    }
    private void OnDisable()
    {
        movementAction.performed -= OnMove;
    }
    #endregion
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        direction.x = Mathf.RoundToInt(direction.x);
        direction.y = Mathf.RoundToInt(direction.y);

        if (moveReset == false)
        {
            PreFire(direction);
            return;
        }
        else
        {
            if (direction.x != 0 && latestPreformedMoveDirection.x != direction.x) { lastDirection = (direction.x > 0) ? MovementDirection.right : MovementDirection.left; }
            if (direction.y != 0 && latestPreformedMoveDirection.y != direction.y) { lastDirection = (direction.y > 0) ? MovementDirection.up : MovementDirection.down; }
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
