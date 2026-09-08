using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float sensitivity = 2f;
    
    private CharacterController controller;
    private Transform cameraTransform;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        Cursor.lockState = CursorLockMode.Locked; // Fareyi ekrana kilitler
    }

    void Update()
    {
        // Fare ile Bakış (Camera Look)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Yukarı/aşağı bakış sınırı

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Hareket (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }
}