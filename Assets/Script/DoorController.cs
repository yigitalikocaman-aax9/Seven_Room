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
    public AudioClip creakSound; 

    private bool isNear = false;
    private bool isOpen = false;
    private Quaternion defaultLocalRotation;
    private Quaternion targetLocalRotation;

    void Start()
    {
        if (doorHinge == null) doorHinge = transform;

        
        defaultLocalRotation = doorHinge.localRotation;
        targetLocalRotation = defaultLocalRotation * Quaternion.Euler(0, openAngle, 0);

        if (ePromptObject != null)
            ePromptObject.SetActive(false);

        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
       
        if (!isLocked && isNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }

        Quaternion target = isOpen ? targetLocalRotation : defaultLocalRotation;
        
       
        doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, target, Time.deltaTime * speed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

       
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

   
    public void UnlockDoor()
    {
        isLocked = false;

        
        if (isNear && ePromptObject != null)
        {
            ePromptObject.SetActive(true);
        }
    }

   
    public void UnlockDoor4()
    {
        UnlockDoor();
        OpenDoor();
    }
}