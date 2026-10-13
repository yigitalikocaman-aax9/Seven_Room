using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("UI / İkon Ayarı")]
    public GameObject ePromptObject;

    [Header("Kilit Sistemi")]
    [Tooltip("Tik işaretliyse kapı kilitli başlar, E butonu gözükmez")]
    public bool isLocked = false; 

    [Header("Kapı / Menteşe Ayarları")]
    public Transform doorHinge;
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Ses Ayarları")]
    public AudioSource audioSource;
    public AudioClip creakSound; // Kapı gıcırdama sesi

    private bool isNear = false;
    private bool isOpen = false;
    private Quaternion defaultLocalRotation;
    private Quaternion targetLocalRotation;

    void Start()
    {
        if (doorHinge == null) doorHinge = transform;

        // Dönüşü local (yerel) eksene sabitle
        defaultLocalRotation = doorHinge.localRotation;
        targetLocalRotation = defaultLocalRotation * Quaternion.Euler(0, openAngle, 0);

        if (ePromptObject != null)
            ePromptObject.SetActive(false);

        // AudioSource atanmamışsa aynı objeden otomatik çek
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Kapı KİLİTLİ DEĞİLSE ve oyuncu yakındaysa E'ye basılınca çalışır
        if (!isLocked && isNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }

        Quaternion target = isOpen ? targetLocalRotation : defaultLocalRotation;
        
        // localRotation kullanarak parent-child çakışmasını engelle
        doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, target, Time.deltaTime * speed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        // Kapı tetiklendiğinde gıcırtı sesini çal
        if (audioSource != null && creakSound != null)
        {
            audioSource.PlayOneShot(creakSound);
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (audioSource != null && creakSound != null)
            {
                audioSource.PlayOneShot(creakSound);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;

            // Sadece kapı kilitli değilse E butonunu göster
            if (!isLocked && ePromptObject != null)
            {
                ePromptObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            if (ePromptObject != null) ePromptObject.SetActive(false);
        }
    }

    // --- NOT OKUNDUĞUNDA ÇAĞRILACAK METHOTLAR ---
    public void UnlockDoor()
    {
        isLocked = false;

        // Eğer oyuncu zaten kapının collider'ı içindeyse E butonunu anında görünür yap
        if (isNear && ePromptObject != null)
        {
            ePromptObject.SetActive(true);
        }
    }

    // 4. Kapı/Siyah kapı için: Hem kilidi açar hem de kapıyı fiziken açar
    public void UnlockDoor4()
    {
        UnlockDoor();
        OpenDoor();
    }
}