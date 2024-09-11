using UnityEditor;
using UnityEngine;
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

    [Header("Gizmos")]
    [SerializeField] LayerMask gizmoLayerMask;
    [SerializeField] float gizmoOffset;
    [SerializeField] float gizmoRadius;

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
        Vector2 moveDirection = Vector2.zero;

        switch (lastDirection)
        {
            case MovementDirection.up:
                moveDirection = Vector2.up;
                break;
            case MovementDirection.right:
                moveDirection = Vector2.right;
                break;
            case MovementDirection.down:
                moveDirection = Vector2.down;
                break;
            case MovementDirection.left:
                moveDirection = Vector2.left;
                break;
        }

        myrigidbody.linearVelocity = moveDirection * movementSpeed;
    }

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

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 overlapDirection = Vector2.zero;

        switch (lastDirection)
        {
            case MovementDirection.up:
                overlapDirection = Vector2.up;
                break;
            case MovementDirection.right:
                overlapDirection = Vector2.right;
                break;
            case MovementDirection.down:
                overlapDirection = Vector2.down;
                break;
            case MovementDirection.left:
                overlapDirection = Vector2.left;
                break;
        }

        if (Physics2D.OverlapCircle((Vector2)transform.position + (overlapDirection * gizmoOffset), gizmoRadius, gizmoLayerMask)) { return; }

        Vector2 direction = context.ReadValue<Vector2>();
        direction.x = Mathf.RoundToInt(direction.x);
        direction.y = Mathf.RoundToInt(direction.y);

        if (direction.x != 0 && lastInput.x != direction.x)
        {
            if (direction.x > 0) { lastDirection = MovementDirection.right; }
            else { lastDirection = MovementDirection.left; }
        }

        if (direction.y != 0 && lastInput.y != direction.y)
        {
            if (direction.y > 0) { lastDirection = MovementDirection.up; }
            else { lastDirection = MovementDirection.down; }
        }

        lastInput = direction;
    }

    public void OnDrawGizmosSelected()
    {
        Vector2 gizmoDirection = Vector2.zero;

        switch (lastDirection)
        {
            case MovementDirection.up:
                gizmoDirection = Vector2.up;
                break;
            case MovementDirection.right:
                gizmoDirection = Vector2.right;
                break;
            case MovementDirection.down:
                gizmoDirection = Vector2.down;
                break;
            case MovementDirection.left:
                gizmoDirection = Vector2.left;
                break;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + (gizmoDirection * gizmoOffset), gizmoRadius);
    }
}
