using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float mouseSensitivity = 3f;
    public float minY = -30f;
    public float maxY = 60f;
    public float collisionPadding = 0.2f;
    public float smoothSpeed = 10f;

    private float yaw;
    private float pitch;
    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = distance;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 direction = rotation * new Vector3(0, 0, -distance);
        Vector3 targetPos = target.position + Vector3.up * 1.5f;

        float adjustedDistance = distance;

        if (Physics.SphereCast(targetPos, 0.3f, direction.normalized, out RaycastHit hit, distance))
        {
            adjustedDistance = hit.distance - collisionPadding;
        }

        // Smooth the distance change
        currentDistance = Mathf.Lerp(currentDistance, adjustedDistance, Time.deltaTime * smoothSpeed);

        transform.position = targetPos + rotation * new Vector3(0, 0, -currentDistance);
        transform.LookAt(targetPos);
    }
}