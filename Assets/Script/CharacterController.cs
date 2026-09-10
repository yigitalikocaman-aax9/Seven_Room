using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Fare Bakış Ayarları")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;
    public float maxLookAngle = 80f;

    [Header("Hareket Ayarları")]
    [Tooltip("Normal yürüme hızı")]
    public float walkSpeed = 5f;

    [Tooltip("Shift'e basılı tutunca koşma hızı")]
    public float runSpeed = 10f;

    [Tooltip("Yerçekimi kuvveti")]
    public float gravity = -19.62f;

    [Tooltip("Zıplama yüksekliği")]
    public float jumpHeight = 1.2f;

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Oyuna girince imleci ekrana kilitler ve gizler
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        if (playerCamera == null) return;

        // Fare hareketlerini al
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Yukarı/Aşağı bakış (Kamera açısı kısıtlaması ile)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Sağa/Sola bakış (Karakter gövdesini çevirir)
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Zeminde mi kontrol et
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD girdilerini al
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Shift basılıysa runSpeed, değilse walkSpeed kullan
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Baktığın yöne göre hareket et
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Zıplama (Space)
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Yerçekimi uygula
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}