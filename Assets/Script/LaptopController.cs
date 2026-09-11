using UnityEngine;

public class LaptopController : MonoBehaviour
{
    [Header("UI Ayarı")]
    public GameObject interactUI;

    [Header("Ekran / Menteşe Ayarları")]
    public Transform screenTransform;
    public float closedAngleX = -110f; // Laptop kapağının öne doğru tam kapanma açısı (-110° veya -90°)
    public float speed = 3f;

    private bool isPlayerNear = false;
    private bool isClosed = false;
    private Quaternion openRotation;
    private Quaternion closedRotation;

    void Start()
    {
        if (screenTransform == null)
            screenTransform = transform;

        // Başlangıçtaki açık durum rotasyonunu kaydet
        openRotation = screenTransform.localRotation;

        // Eksi X ekseni yönünde kapanma açısını hesapla
        closedRotation = openRotation * Quaternion.Euler(closedAngleX, 0f, 0f);

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            isClosed = !isClosed;
        }

        Quaternion targetRotation = isClosed ? closedRotation : openRotation;
        screenTransform.localRotation = Quaternion.Slerp(screenTransform.localRotation, targetRotation, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}