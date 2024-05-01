using UnityEngine;

public class CameraRotationService : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;
    [SerializeField]
    private float rotationSpeed = 50f;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = transform;
    }

    public void RotateCamera(float mouseX, float mouseY)
    {
        float rotationX = -mouseY * rotationSpeed * Time.deltaTime;
        float rotationY = mouseX * rotationSpeed * Time.deltaTime;
        cameraTransform.RotateAround(targetObject.position, cameraTransform.right, rotationX);
        cameraTransform.RotateAround(targetObject.position, Vector3.up, rotationY);
    }
}
