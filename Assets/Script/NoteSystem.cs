using UnityEngine;
using TMPro;

public class NoteSystem : MonoBehaviour
{
    [Header("UI Kutucuklari")]
    [Tooltip("Kagida yaklasinca ekranda cikacak E harfi")]
    public GameObject interactPromptUI; 

    [Tooltip("Kagidin kendisinin ve metninin oldugu Buyuk Not Paneli")]
    public GameObject notePanelUI; 

    [Tooltip("Not panelinin icindeki TextMeshPro metin alani")]
    public TMP_Text noteTextUI; 

    [Header("Not Icerigi")]
    [TextArea(5, 10)]
    public string noteContent = "Buraya okutmak istedigin notu yaz...";

    [Header("Acilacak Kapi Baglantisi")]
    [Tooltip("Bu not okununca kilidi acilacak olan kapi")]
    public DoorController doorToUnlock;

    private bool isPlayerInsideTrigger = false;
    private bool isNoteOpen = false;

    private void Start()
    {
        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;

            if (!isNoteOpen && interactPromptUI != null)
            {
                interactPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;

            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Collider icindeyken E'ye basinca NOTU AC
        if (isPlayerInsideTrigger && !isNoteOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
        // Not aciksa ESC'ye basinca NOTU KAPAT
        else if (isNoteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    private void OpenNote()
    {
        isNoteOpen = true;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(true);
        if (noteTextUI != null) noteTextUI.text = noteContent;

        // Oyunu tamamen durdur
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseNote()
    {
        isNoteOpen = false;

        if (notePanelUI != null) notePanelUI.SetActive(false);

        // Oyunu devam ettir
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (isPlayerInsideTrigger && interactPromptUI != null)
        {
            interactPromptUI.SetActive(true);
        }

        // --- NOT KAPANDI, 2. KAPININ KİLİDİNİ AÇ ---
        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor();
        }
    }
}