using UnityEngine;
using TMPro;

public class NoteSystem : MonoBehaviour
{
    [Header("UI Kutucuklari")]
    public GameObject interactPromptUI; 
    public GameObject notePanelUI; 
    public TMP_Text noteTextUI; 

    [Header("Not Icerigi")]
    [TextArea(5, 10)]
    public string noteContent = "Buraya okutmak istedigin notu yaz...";

    [Header("Gerekli Baglantilar")]
    public DoorController doorToUnlock;
    public PlateInteraction plateScript;             // Tabak Script'i (Varsa)
    public LaptopInteraction laptopScript;           // Laptop Script'i (Varsa)
    public LightswitchInteraction lightswitchScript; // Işık Düğmesi Script'i (Varsa)
    public YorganInteraction yorganScript;           // Yorgan Script'i (Varsa)

    private bool isPlayerInsideTrigger = false;
    private bool isNoteOpen = false;
    private bool isAlreadyRead = false;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
            if (!isAlreadyRead && !isNoteOpen && interactPromptUI != null)
            {
                interactPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isAlreadyRead && isPlayerInsideTrigger && !isNoteOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenNote();
        }
        else if (isNoteOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            CloseNote();
        }

        if (!isAlreadyRead && !isNoteOpen && Input.GetMouseButtonDown(0))
        {
            CheckDirectClick();
        }
    }

    private void CheckDirectClick()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                OpenNote();
                break;
            }
        }
    }

    public void OpenNote()
    {
        if (isAlreadyRead) return;

        isNoteOpen = true;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (notePanelUI != null) notePanelUI.SetActive(true);
        if (noteTextUI != null) noteTextUI.text = noteContent;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNote()
    {
        isNoteOpen = false;
        isAlreadyRead = true;

        if (notePanelUI != null) notePanelUI.SetActive(false);
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // TABAK GERİ İNSİN
        if (plateScript != null)
        {
            plateScript.LowerPlate();
        }

        // LAPTOP GERİ İNSİN
        if (laptopScript != null)
        {
            laptopScript.LowerLaptop();
        }

        // IŞIK DÜĞMESİ GERİ ÇEKİLSİN
        if (lightswitchScript != null)
        {
            lightswitchScript.MoveBack();
        }

        // YORGAN GERİ İNSİN
        if (yorganScript != null)
        {
            yorganScript.LowerYorgan();
        }

        if (doorToUnlock != null)
        {
            doorToUnlock.UnlockDoor4();
            Debug.Log("Not okundu. Obje yerine çekildi ve kapı açıldı!");
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}