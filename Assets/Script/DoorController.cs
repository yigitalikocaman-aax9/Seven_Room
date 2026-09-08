using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;   // Açılma açısı (Dışarı/içeri yönüne göre -90 da yapılabilir)
    [SerializeField] private float openSpeed = 3f;    // Açılma hızı
    [SerializeField] private float interactionDistance = 3f; // E tuşuna basabilmek için maksimum mesafe

    [Header("UI & Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform playerTransform;

    private void Start()
    {
        // Kapının başlangıç rotasyonunu kaydet
        closedRotation = transform.localRotation;
        // Hedef açık rotasyonu hesapla (Y ekseninde etrafında döndürme)
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);

        // Sahnedeki Player objesini Tag üzerinden bul
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Karakter ile kapı arasındaki mesafeyi ölç
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        // Yakındaysa ve E tuşuna basıldıysa kapı durumunu değiştir
        if (distance <= interactionDistance && Input.GetKeyDown(interactKey))
        {
            isOpen = !isOpen;
        }

        // Kapıyı yeni açısına Lerp (yumuşak geçiş) ile döndür
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);
    }

    // Scene ekranında etkileşim mesafesini sarı küre olarak görmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}