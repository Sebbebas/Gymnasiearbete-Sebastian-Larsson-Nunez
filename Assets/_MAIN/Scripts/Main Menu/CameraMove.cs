using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 5f;

    void Update()
    {
        Vector3 currentPosition = transform.position;

        Vector3 targetPosition = currentPosition + Vector3.right * speedMultiplier * Time.deltaTime;

        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.1f);
    }
}
