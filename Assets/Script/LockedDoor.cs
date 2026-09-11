using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioSource audioSource;
    public AudioClip lockedSound;

    [Header("UI Ayarları")]
    public GameObject interactUI;

    private bool isPlayerInTrigger = false;

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            PlayLockedSound();
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

    void PlayLockedSound()
    {
        if (audioSource != null && lockedSound != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }
    }
}