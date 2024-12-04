using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] MovementDirection lastPerformedMovementDirection;
    [SerializeField] Vector2 lastPerformedVector2;

    PlayerMovement playerMovement;
    Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        lastPerformedVector2 = playerMovement.GetlastPreformedVector2();
        lastPerformedMovementDirection = playerMovement.GetlastPreformedInput();

        switch (lastPerformedMovementDirection)
        {
            case MovementDirection.up:
                ResetAnimationTriggers();
                animator.SetTrigger("Up");
                break;
            case MovementDirection.down:
                ResetAnimationTriggers();
                animator.SetTrigger("Down");
                break;
            case MovementDirection.left:
                ResetAnimationTriggers();
                animator.SetTrigger("Left");
                break;
            case MovementDirection.right:
                ResetAnimationTriggers();
                animator.SetTrigger("Right");
                break;
        }
    }
    private void ResetAnimationTriggers()
    {
        animator.ResetTrigger("Up");
        animator.ResetTrigger("Down");
        animator.ResetTrigger("Left");
        animator.ResetTrigger("Right");
    }
}
