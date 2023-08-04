using UnityEngine;

public class SnapToBottom : MonoBehaviour
{
    public Transform targetObject; // The object to snap to

    private void Update()
    {
        SnapToBottomPosition();

    }

    private void SnapToBottomPosition()
    {
        // Calculate the position to snap to
        Vector3 snapPosition = CalculateSnapPosition();

        // Calculate the offset between the current object's position and the target's bottom position
        Vector3 offset = snapPosition - GetTopPosition();

        // Apply the offset to move the object to the snapped position
        transform.position += offset;
    }

    private Vector3 CalculateSnapPosition()
    {
        // Calculate the snap position at the bottom of the target object
        Vector3 targetBounds = targetObject.GetComponent<Renderer>().bounds.size;
        Vector3 targetPosition = targetObject.position;
        Vector3 snapPosition = targetPosition - targetObject.up * (targetBounds.y * 0.5f);

        return snapPosition;
    }

    private Vector3 GetTopPosition()
    {
        // Calculate the position at the top of the current object
        Vector3 objectBounds = GetComponent<Renderer>().bounds.size;
        Vector3 objectPosition = transform.position;
        Vector3 topPosition = objectPosition + transform.up * (objectBounds.y * 0.5f);

        return topPosition;
    }
}
