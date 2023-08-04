using UnityEngine;

public class SnapToTarget : MonoBehaviour
{
    public Transform targetObject; // The object to snap to
    float height;
    public bool isTop = false; // Determines whether to snap to the top or bottom

    private void Start()
    {
        height = GameData.Data.Gaps.GapHeight;
    }

    private void Update()
    {
        SnapToTargetPosition();
    }

    private void SnapToTargetPosition()
    {
        // Calculate the position to snap to
        Vector3 snapPosition = CalculateSnapPosition();

        // Calculate the offset between the current object's position and the target's position
        Vector3 offset = snapPosition - GetReferencePosition();

        // Apply the offset to move the object to the snapped position
        transform.position += offset;
    }

    private Vector3 CalculateSnapPosition()
    {
        // Calculate the snap position based on the selected option (top or bottom)
        //Vector3 targetBounds = targetObject.GetComponent<Renderer>().bounds.size;
        Vector3 targetBounds = targetObject.GetComponent<Collider2D>().bounds.size;
        Vector3 targetPosition = targetObject.position;

        Vector3 snapPosition;

        if (isTop)
        {
            snapPosition = targetPosition + targetObject.up * (targetBounds.y * 0.5f);
            //snapPosition = targetPosition + targetObject.up * ((height + 1) * 0.5f);
            //snapPosition = targetPosition + targetObject.up * (height / 2 + 0.5f);
        }
        else
        {
            snapPosition = targetPosition - targetObject.up * (targetBounds.y * 0.5f);
            //snapPosition = targetPosition - targetObject.up * ((height + 1) * 0.5f);
            //snapPosition = targetPosition - targetObject.up * (height / 2 + 0.5f);


        }

        // Adjust the snap position if the top object is upside down
        if (isTop && targetObject.rotation.eulerAngles.z == 180f)
        {
            snapPosition += targetObject.up * targetBounds.y;
            //snapPosition += targetObject.up * (height + 1);
            //snapPosition += targetObject.up * (height);
        }

        return snapPosition;
    }

    private Vector3 GetReferencePosition()
    {
        // Calculate the reference position based on the selected option (top or bottom)
        // Vector3 objectBounds = GetComponent<Renderer>().bounds.size;
        Vector3 objectBounds = GetComponent<Collider2D>().bounds.size;
        Vector3 objectPosition = transform.position;

        Vector3 referencePosition;


        if (isTop)
        {
            referencePosition = objectPosition - transform.up * (objectBounds.y * 0.5f);
            // referencePosition = objectPosition - transform.up * ((height + 1) * 0.5f);
            //referencePosition = objectPosition - transform.up * (height / 2);

        }
        else
        {
            referencePosition = objectPosition + transform.up * (objectBounds.y * 0.5f);
            //referencePosition = objectPosition + transform.up * ((height + 1) * 0.5f);
            //referencePosition = objectPosition + transform.up * (height / 2);
        }


        // Adjust the reference position if the top object is upside down
        if (isTop && transform.rotation.eulerAngles.z == 180f)
        {
            referencePosition += transform.up * objectBounds.y;
            //referencePosition += transform.up * (height + 1);
            //referencePosition += transform.up * (height);
        }


        return referencePosition;
    }
}
