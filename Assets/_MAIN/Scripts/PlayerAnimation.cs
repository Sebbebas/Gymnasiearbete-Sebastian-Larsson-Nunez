using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Vector2 currentMoveDirection;

    PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        currentMoveDirection = playerMovement.GetCurrentMoveDirecion();

        transform.localRotation = new Quaternion(currentMoveDirection.y, 0, 0, 0);
    }
}
