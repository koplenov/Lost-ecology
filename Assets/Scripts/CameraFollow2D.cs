using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public Transform Target;

    private void FixedUpdate()
    {
        Vector3 newPosition = Target.position;
        newPosition.z = -10;
        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
    }
}