using System.Collections;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioSource audioSource;
    public AudioClip lockedSound;       // 1. Ses: Tak tak tak (Tıklama / Kilit sesi)
    public AudioClip secondSound;       // 2. Ses: "Kim o?", konuşma veya korku efekti (Sadece 1 kez çalar)

    [Header("Ses Seviyesi Ayarları")]
    [Range(0f, 2f)]
    [Tooltip("2. Sesin yüksekliği (1.0 normal seviyedir, 1.2 veya 1.5 yaparak sesini artırabilirsin)")]
    public float secondSoundVolume = 1.3f; 

    [Header("Zamanlama Ayarı")]
    [Tooltip("İlk ses başladıktan kaç saniye sonra 2. ses çalsın?")]
    public float secondSoundDelay = 1.0f; 

    [Header("UI Ayarları")]
    public GameObject interactUI;

    private bool isPlayerInTrigger = false;
    private bool isPlayingSequence = false; // Sesler çalarken tekrar E'ye basılmasını engeller
    private bool hasSecondSoundPlayed = false; // 2. sesin sadece 1 kere çalmasını sağlar

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        // Oyuncu alandaysa, E'ye bastıysa ve halihazırda ses sekansı çalmıyorsa başlat
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isPlayingSequence)
        {
            StartCoroutine(PlayDoorSoundSequence());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    // Sıralı ses çalma mekanizması (Coroutine)
    IEnumerator PlayDoorSoundSequence()
    {
        isPlayingSequence = true;

        // 1. İlk sesi çal (Tak tak tak - Her E'ye basıldığında çalar)
        if (audioSource != null && lockedSound != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }

        // Eğer 2. ses daha önce ÇALINMADIYSA ve tanımlıysa çalıştır
        if (!hasSecondSoundPlayed && secondSound != null)
        {
            hasSecondSoundPlayed = true; // 2. sesin tekrar çalınmasını engellemek için işaretle

            // Belirlediğin süre kadar bekle
            yield return new WaitForSeconds(secondSoundDelay);

            // 2. İkinci sesi artırılmış ses seviyesiyle çal
            if (audioSource != null)
            {
                audioSource.PlayOneShot(secondSound, secondSoundVolume);
            }

            // İkinci sesin bitmesi için bekle
            yield return new WaitForSeconds(secondSound.length);
        }
        else if (lockedSound != null)
        {
            // 2. ses zaten çalındıysa, sadece ilk sesin bitmesini bekle
            yield return new WaitForSeconds(lockedSound.length);
        }

        isPlayingSequence = false;
    }

    void PlayLockedSound()
    {
        if (!isPlayingSequence)
        {
            StartCoroutine(PlayDoorSoundSequence());
        }
    }
}